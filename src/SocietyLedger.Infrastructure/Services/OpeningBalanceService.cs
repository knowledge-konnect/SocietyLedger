using Microsoft.EntityFrameworkCore;
using Serilog;
using SocietyLedger.Application.DTOs.OpeningBalance;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Services
{
    public class OpeningBalanceService : IOpeningBalanceService
    {
        private readonly AppDbContext _db;

        public OpeningBalanceService(AppDbContext db)
        {
            _db = db;
        }

        // Carries the minimum status metadata needed from each opening-balance source.
        private sealed record OpeningMeta(DateOnly TransactionDate, DateTime AuditCreatedAt, long CreatedBy);

        public async Task<OpeningBalanceStatusResponse> GetStatusAsync(long societyId)
        {
            // member-side opening (adjustments table)
            var memberOpening = await _db.adjustments
                .Where(a => a.society_id == societyId
                         && a.entry_type == EntryTypeCodes.OpeningBalance
                         && !a.is_deleted)
                // Order by the business date so that the "earliest" financial event
                // is selected, not the earliest row insertion.
                .OrderBy(a => a.created_at)   // adjustments table has no transaction_date yet
                .Select(a => new OpeningMeta(
                    DateOnly.FromDateTime(a.created_at),   // fallback: derive from created_at
                    a.created_at,
                    a.created_by ?? 0
                ))
                .FirstOrDefaultAsync();

            // society-side opening (fund ledger)
            var societyOpening = await _db.society_fund_ledgers
                .Where(f => f.society_id == societyId
                         && f.entry_type == EntryTypeCodes.OpeningFund
                         && !f.is_deleted!.Value)
                // Use transaction_date for financial ordering — NOT created_at.
                .OrderBy(f => f.transaction_date)
                .Select(f => new OpeningMeta(
                    f.transaction_date ?? DateOnly.FromDateTime(f.created_at),
                    f.created_at,
                    f.created_by
                ))
                .FirstOrDefaultAsync();

            var applied = memberOpening != null || societyOpening != null;

            if (!applied)
            {
                return new OpeningBalanceStatusResponse
                {
                    IsApplied = false,
                    TransactionDate = null,
                    AuditCreatedAt = null,
                    AppliedBy = null
                };
            }

            // Prefer the earliest financial date across both sources.
            var source = (memberOpening, societyOpening) switch
            {
                ({ } m, null)   => m,
                (null, { } s)   => s,
                ({ } m, { } s)  => m.TransactionDate <= s.TransactionDate ? m : s,
                _               => throw new InvalidOperationException("Unreachable")
            };

            var userName = await _db.users
                .Where(u => u.id == source.CreatedBy)
                .Select(u => u.name)
                .FirstOrDefaultAsync();

            return new OpeningBalanceStatusResponse
            {
                IsApplied = true,
                TransactionDate = source.TransactionDate,
                AuditCreatedAt = source.AuditCreatedAt,
                AppliedBy = userName
            };
        }

        public async Task ApplyOpeningBalanceAsync(
            OpeningBalanceRequest request,
            long societyId,
            long userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // ── 1. Load the society to resolve onboarding_date for date validation ─
            var society = await _db.societies
                .AsNoTracking()
                .Where(s => s.id == societyId && !s.is_deleted)
                .Select(s => new { s.onboarding_date })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Society", societyId.ToString());

            // ── 2. Validate transaction_date against financial epoch ──────────────
            if (request.TransactionDate < society.onboarding_date)
                throw new ValidationException(
                    $"transaction_date ({request.TransactionDate:yyyy-MM-dd}) cannot be earlier than " +
                    $"the society onboarding date ({society.onboarding_date:yyyy-MM-dd}).");

            if (request.TransactionDate > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ValidationException(
                    $"transaction_date ({request.TransactionDate:yyyy-MM-dd}) cannot be in the future.");

            // ── 3. Duplicate guard (application-level for friendly error messages) ─
            //      The DB also enforces this via the uq_society_single_opening partial
            //      unique index as a safety net against concurrent inserts.
            var memberOpeningExists = await _db.adjustments
                .AnyAsync(a => a.society_id == societyId
                            && a.entry_type == EntryTypeCodes.OpeningBalance
                            && !a.is_deleted);

            var societyOpeningExists = await _db.society_fund_ledgers
                .AnyAsync(f => f.society_id == societyId
                            && f.entry_type == EntryTypeCodes.OpeningFund
                            && !f.is_deleted!.Value);

            if (memberOpeningExists || societyOpeningExists)
                throw new ValidationException(
                    "Opening balance has already been applied for this society. " +
                    "It can only be set once.");

            // ── 4. Require at least one non-zero value ────────────────────────────
            var hasFlatItems    = request.flat_items?.Any(i => i.Amount != 0) == true;
            var hasSocietyAmount = request.society_opening_amount > 0;

            if (!hasFlatItems && !hasSocietyAmount)
                throw new ValidationException(
                    "At least one opening value must be provided (society amount or a flat amount).");

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                // ── 5a. Member opening dues (adjustments table) ───────────────────
                if (hasFlatItems)
                {
                    var flatPublicIds = request.flat_items!
                        .Where(i => i.Amount != 0)
                        .Select(i => i.FlatPublicId)
                        .ToList();

                    var flats = await _db.flats
                        .Where(f => flatPublicIds.Contains(f.public_id)
                                 && f.society_id == societyId)
                        .ToDictionaryAsync(f => f.public_id);

                    foreach (var item in request.flat_items!.Where(i => i.Amount != 0))
                    {
                        if (!flats.TryGetValue(item.FlatPublicId, out var flat))
                            throw new ValidationException(
                                $"Flat {item.FlatPublicId} not found or does not belong to this society.");

                        _db.adjustments.Add(new adjustment
                        {
                            society_id        = societyId,
                            flat_id           = flat.id,
                            amount            = item.Amount,
                            remaining_amount  = item.Amount,
                            reason            = "Opening Balance - Migration",
                            created_by        = userId,
                            entry_type        = EntryTypeCodes.OpeningBalance,
                            created_at        = now,   // audit only
                            is_deleted        = false
                        });
                    }
                }

                // ── 5b. Society opening fund (bank balance — fund ledger) ─────────
                if (hasSocietyAmount)
                {
                    _db.society_fund_ledgers.Add(BuildLedgerEntry(
                        societyId:       societyId,
                        amount:          request.society_opening_amount,
                        entryType:       EntryTypeCodes.OpeningFund,
                        transactionDate: request.TransactionDate,
                        notes:           "Opening bank balance during migration",
                        reference:       null,
                        userId:          userId,
                        auditNow:        now));
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                Log.Information(
                    "Opening balance applied for society {SocietyId}. " +
                    "TransactionDate: {TxDate}, FlatItems: {FlatCount}, SocietyFund: {Amount}",
                    societyId,
                    request.TransactionDate,
                    request.flat_items?.Count(i => i.Amount != 0) ?? 0,
                    request.society_opening_amount);
            }
            catch
            {
                await transaction.RollbackAsync();
                Log.Error("Error applying opening balance for society {SocietyId}", societyId);
                throw;
            }
        }

        // ── Private factory ───────────────────────────────────────────────────────
        // Centralises society_fund_ledger creation so that every callsite is forced
        // to supply transaction_date explicitly.  created_at is set from auditNow and
        // is NEVER used as the financial date.
        private static society_fund_ledger BuildLedgerEntry(
            long     societyId,
            decimal  amount,
            string   entryType,
            DateOnly transactionDate,
            string?  notes,
            string?  reference,
            long     userId,
            DateTime auditNow) => new()
        {
            public_id        = Guid.NewGuid(),
            society_id       = societyId,
            amount           = amount,
            entry_type       = entryType,
            transaction_date = transactionDate,   // ← financial event date
            created_at       = auditNow,          // ← audit timestamp only
            reference        = reference,
            notes            = notes,
            created_by       = userId,
            is_deleted       = false
        };
    }
}

