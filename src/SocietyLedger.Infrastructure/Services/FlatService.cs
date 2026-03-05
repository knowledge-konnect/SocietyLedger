using SocietyLedger.Application.DTOs.Flat;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Services
{
    public class FlatService : IFlatService
    {
        private readonly IFlatRepository _repo;
        private readonly IUserContext _userContext;
        private readonly AppDbContext _db;
        private readonly ILogger<FlatService> _logger;

        public FlatService(
            IFlatRepository repo, 
            IUserContext userContext,
            AppDbContext db,
            ILogger<FlatService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<FlatResponseDto>> GetBySocietyIdAsync(long societyId)
        {
            var list = await _repo.GetBySocietyIdAsync(societyId);
            return list.Select(f => MapToDto(f));
        }
        /// <summary>
        /// Create a new flat and return the created DTO.
        /// </summary>
        public async Task<FlatResponseDto> CreateAsync(CreateFlatDto dto, long userId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var societyId = await _userContext.GetSocietyIdAsync(userId);

            // Check for duplicate flat number in the society
            var existingFlat = await _repo.GetByFlatNoAndSocietyAsync(dto.FlatNo, societyId);
            if (existingFlat != null)
                throw new DuplicateException("flat", "flat number");

            // Get status by code if provided, otherwise use default
            short? statusId = null;
            if (!string.IsNullOrEmpty(dto.StatusCode))
            {
                var status = await _repo.GetByCodeAsync(dto.StatusCode);
                if (status == null)
                    throw new ValidationException($"Invalid flat status code: {dto.StatusCode}");
                statusId = status.Id;
            }

            var now = DateTime.UtcNow;

            var domain = new Flat
            {
                PublicId = Guid.NewGuid(),
                SocietyId = societyId,
                FlatNo = dto.FlatNo,
                OwnerName = dto.OwnerName,
                ContactMobile = dto.ContactMobile,
                ContactEmail = dto.ContactEmail,
                MaintenanceAmount = dto.MaintenanceAmount ?? 0m,
                StatusId = statusId,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _repo.AddAsync(domain);
            await _repo.SaveChangesAsync();

            _logger.LogInformation("Flat created successfully for FlatNo {FlatNo}", dto.FlatNo);
            return MapToDto(domain);
        }
        

        /// <summary>
        /// Get a flat by its public UUID with tenant isolation.
        /// </summary>
        public async Task<FlatResponseDto?> GetByPublicIdAsync(Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var domain = await _repo.GetByPublicIdAsync(publicId, societyId);
            if (domain == null)
                throw new NotFoundException("Flat", publicId.ToString());
            return MapToDto(domain);
        }

        /// <summary>
        /// Update an existing flat with tenant isolation.
        /// Returns the updated DTO if found, or null if not.
        /// </summary>
        public async Task<FlatResponseDto?> UpdateAsync(UpdateFlatDto dto, long userId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var existing = await _repo.GetByPublicIdAsync(dto.PublicId, societyId);
            if (existing == null)
                throw new NotFoundException("Flat", dto.PublicId.ToString());

            // Check for duplicate flat number if changing
            if (dto.FlatNo != null && dto.FlatNo != existing.FlatNo)
            {
                var conflictingFlat = await _repo.GetByFlatNoAndSocietyAsync(dto.FlatNo, existing.SocietyId);
                if (conflictingFlat != null)
                    throw new DuplicateException("flat", "flat number");
            }

            // Get status by code if provided
            short? statusId = existing.StatusId;
            if (dto.StatusCode != null)
            {
                var status = await _repo.GetByCodeAsync(dto.StatusCode);
                if (status == null)
                    throw new ValidationException($"Invalid flat status code: {dto.StatusCode}");
                statusId = status.Id;
            }

            existing.FlatNo = dto.FlatNo ?? existing.FlatNo;
            existing.OwnerName = dto.OwnerName ?? existing.OwnerName;
            existing.ContactMobile = dto.ContactMobile ?? existing.ContactMobile;
            existing.ContactEmail = dto.ContactEmail ?? existing.ContactEmail;
            existing.MaintenanceAmount = dto.MaintenanceAmount ?? existing.MaintenanceAmount;
            existing.StatusId = statusId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing, societyId);
            await _repo.SaveChangesAsync();

            _logger.LogInformation("Flat updated successfully for PublicId {PublicId}", dto.PublicId);
            return MapToDto(existing);
        }
        

        /// <summary>
        /// Delete a flat by its public UUID with tenant isolation.
        /// Returns true if deleted, false if not found.
        /// </summary>
        public async Task<bool> DeleteByPublicIdAsync(Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var existing = await _repo.GetByPublicIdAsync(publicId, societyId);
            if (existing == null)
                throw new NotFoundException("Flat", publicId.ToString());

            await _repo.DeleteByPublicIdAsync(publicId, societyId);
            await _repo.SaveChangesAsync();
            _logger.LogInformation("Flat deleted successfully for PublicId {PublicId}", publicId);
            return true;
        }

       
        public async Task<IEnumerable<FlatStatusDto>> GetAllAsync()
        {
            var statuses = await _repo.GetAllAsync();
            return statuses.Select(s => new FlatStatusDto(s.Code, s.DisplayName));
        }

        public async Task<FlatLedgerResponse> GetFlatLedgerAsync(Guid publicId, long userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            // Get flat by publicId and verify it belongs to user's society
            var flat = await _db.flats.FirstOrDefaultAsync(f => f.public_id == publicId && f.society_id == societyId);
            if (flat == null)
                throw new NotFoundException("Flat", publicId.ToString());

            var flatId = flat.id;
            var ledgerEntries = new List<FlatLedgerEntryDto>();

            // Get all adjustments for the flat (maintenance charges, opening balance, etc.)
            var adjustmentsQuery = _db.adjustments
                .Where(a => a.flat_id == flatId && !a.is_deleted);

            if (startDate.HasValue)
                adjustmentsQuery = adjustmentsQuery.Where(a => a.created_at >= startDate.Value);
            if (endDate.HasValue)
                adjustmentsQuery = adjustmentsQuery.Where(a => a.created_at <= endDate.Value);

            var adjustments = await adjustmentsQuery
                .OrderBy(a => a.created_at)
                .ToListAsync();

            // Get all payments for the flat
            var paymentsQuery = _db.maintenance_payments
                .Where(p => p.flat_id == flatId && !p.is_deleted);

            if (startDate.HasValue)
                paymentsQuery = paymentsQuery.Where(p => p.payment_date >= startDate.Value);
            if (endDate.HasValue)
                paymentsQuery = paymentsQuery.Where(p => p.payment_date <= endDate.Value);

            var payments = await paymentsQuery
                .OrderBy(p => p.payment_date)
                .ToListAsync();

            // Calculate opening balance (sum of adjustments before startDate minus payments before startDate)
            decimal openingBalance = 0;
            if (startDate.HasValue)
            {
                var adjustmentsBefore = await _db.adjustments
                    .Where(a => a.flat_id == flatId && a.created_at < startDate.Value && !a.is_deleted)
                    .SumAsync(a => (decimal?)a.amount) ?? 0;

                var paymentsBefore = await _db.maintenance_payments
                    .Where(p => p.flat_id == flatId && p.payment_date < startDate.Value && !p.is_deleted)
                    .SumAsync(p => (decimal?)p.amount) ?? 0;

                openingBalance = adjustmentsBefore - paymentsBefore;
            }

            // Add adjustment entries
            foreach (var adj in adjustments)
            {
                // Extract period from reason if it's monthly maintenance
                string? period = null;
                if (adj.entry_type == EntryTypeCodes.MonthlyMaintenance && adj.reason != null && adj.reason.Contains(" - "))
                {
                    var parts = adj.reason.Split(" - ");
                    if (parts.Length > 1)
                        period = parts[1];
                }

                ledgerEntries.Add(new FlatLedgerEntryDto
                {
                    Date = adj.created_at,
                    EntryType = "maintenance",
                    Period = period,
                    Charge = adj.amount,
                    Payment = 0,
                    Balance = 0, // Will be calculated below
                    Description = adj.reason,
                    ReferenceNumber = null
                });
            }

            // Add payment entries
            foreach (var pmt in payments)
            {
                ledgerEntries.Add(new FlatLedgerEntryDto
                {
                    Date = pmt.payment_date,
                    EntryType = "payment",
                    Period = null, // Payments are not linked to specific period
                    Charge = 0,
                    Payment = pmt.amount,
                    Balance = 0, // Will be calculated below
                    Description = $"Payment - {pmt.notes ?? ""}",
                    ReferenceNumber = pmt.reference_number
                });
            }

            // Sort all entries by date and calculate running balance
            ledgerEntries = ledgerEntries.OrderBy(e => e.Date).ToList();
            decimal runningBalance = openingBalance;

            foreach (var entry in ledgerEntries)
            {
                runningBalance += entry.Charge - entry.Payment;
                entry.Balance = runningBalance;
            }

            _logger.LogInformation("Flat ledger retrieved for flat {PublicId}", publicId);

            return new FlatLedgerResponse
            {
                FlatPublicId = flat.public_id,
                FlatNo = flat.flat_no,
                OwnerName = flat.owner_name,
                OpeningBalance = openingBalance,
                ClosingBalance = runningBalance,
                Entries = ledgerEntries
            };
        }

        public async Task<FlatFinancialSummaryResponse> GetFlatFinancialSummaryAsync(Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var flat = await _db.flats
                .FirstOrDefaultAsync(f => f.public_id == publicId
                                       && f.society_id == societyId
                                       && !f.is_deleted);

            if (flat == null)
                throw new NotFoundException("Flat", publicId.ToString());

            //  Opening Balance Remaining (NOT original amount)
            var openingBalanceRemaining = await _db.adjustments
                .Where(a => a.flat_id == flat.id
                         && a.entry_type == EntryTypeCodes.OpeningBalance
                         && !a.is_deleted)
                .SumAsync(a => (decimal?)a.remaining_amount) ?? 0;

            //  Bill Outstanding
            var billOutstanding = await _db.bills
                .Where(b => b.flat_id == flat.id
                         && !b.is_deleted)
                .SumAsync(b => (decimal?)(b.amount - b.paid_amount)) ?? 0;

            // Total Charges (optional for UI)
            var totalCharges = await _db.bills
                .Where(b => b.flat_id == flat.id && !b.is_deleted)
                .SumAsync(b => (decimal?)b.amount) ?? 0;

            // Total Payments (for display only)
            var totalPayments = await _db.maintenance_payments
                .Where(p => p.flat_id == flat.id && !p.is_deleted)
                .SumAsync(p => (decimal?)p.amount) ?? 0;

            var totalOutstanding = openingBalanceRemaining + billOutstanding;

            _logger.LogInformation("Financial summary retrieved for flat {PublicId}", publicId);

            return new FlatFinancialSummaryResponse
            {
                OpeningBalanceRemaining = openingBalanceRemaining,
                BillOutstanding = billOutstanding,
                TotalOutstanding = totalOutstanding,
                TotalCharges = totalCharges,
                TotalPayments = totalPayments
            };
        }


        #region Mapping helpers

        private static FlatResponseDto MapToDto(Flat f)
        {
            return new FlatResponseDto(
                f.PublicId,
                f.SocietyPublicId,
                f.FlatNo,
                f.OwnerName,
                f.ContactMobile,
                f.ContactEmail,
                f.MaintenanceAmount,
                f.StatusId,
                f.StatusName,
                f.CreatedAt,
                f.UpdatedAt
            );
        }

        #endregion
    }
}
