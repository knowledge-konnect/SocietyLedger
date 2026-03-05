using Microsoft.EntityFrameworkCore;
using Serilog;
using SocietyLedger.Application.DTOs.MaintenancePayment;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Services.Common;
using System.Text.RegularExpressions;

namespace SocietyLedger.Infrastructure.Services
{
    public class MaintenancePaymentService : IMaintenancePaymentService
    {
        // Bill status codes sourced from BillStatusCodes (Domain.Constants) — single source of truth.

        private readonly IMaintenancePaymentRepository _maintenancePaymentRepo;
        private readonly IPaymentModeRepository _paymentModeRepo;
        private readonly IUserContext _userContext;
        private readonly AppDbContext _db;
        private readonly IDapperService _dapper;

        public MaintenancePaymentService(
            IMaintenancePaymentRepository maintenancePaymentRepo,
            IPaymentModeRepository paymentModeRepo,
            IUserContext userContext,
            AppDbContext db,
            IDapperService dapper)
        {
            _maintenancePaymentRepo = maintenancePaymentRepo;
            _paymentModeRepo        = paymentModeRepo;
            _userContext            = userContext;
            _db                     = db;
            _dapper                 = dapper;
        }

        // ------------------------------------------------------------------ //
        //  FIFO payment allocation                                             //
        //                                                                      //
        //  Allocation order (all within a single RepeatableRead transaction):  //
        //    1. OpeningBalance adjustments  — oldest created_at first          //
        //    2. Monthly bills               — oldest period first              //
        //    3. Remainder → advance row     — bill_id=NULL, adjustment_id=NULL //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Records a maintenance payment and allocates it FIFO:
        /// opening-balance adjustments first, then monthly bills, then advance.
        /// </summary>
        public async Task<MaintenancePaymentResponse> ProcessPaymentAsync(
            MaintenancePaymentRequest request,
            long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            if (request.Amount <= 0)
                throw new ValidationException("Amount must be positive.");

            // Validate business date against the society's financial epoch.
            // This must happen before the Dapper transaction to keep error reporting clean.
            var onboardingDate = await GetOnboardingDateAsync(societyId);
            var paymentDateOnly = DateOnly.FromDateTime(request.PaymentDate);
            if (paymentDateOnly < onboardingDate)
                throw new ValidationException(
                    $"Payment date ({paymentDateOnly:yyyy-MM-dd}) cannot be earlier than " +
                    $"the society onboarding date ({onboardingDate:yyyy-MM-dd}).");

            var idempotencyKey = string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? Guid.NewGuid().ToString()
                : request.IdempotencyKey;

            var (conn, tx) = await _dapper.BeginTransactionAsync();
            await using (conn)
            await using (tx)
            {
                try
                {
                    // ── Step 1: Idempotency check ─────────────────────────────────────────
                    var existing = await _dapper.QueryAsync<dynamic>(
                        conn, tx,
                        SqlQueries.CheckMaintenancePaymentIdempotency,
                        new { SocietyId = societyId, IdempotencyKey = idempotencyKey });

                    if (existing.Any())
                    {
                        var rows = (await _dapper.QueryAsync<dynamic>(
                            conn, tx,
                            SqlQueries.GetAllocationsByIdempotencyKey,
                            new { SocietyId = societyId, IdempotencyKey = idempotencyKey })).ToList();

                        // Bill allocations — rows where bill_public_id is populated.
                        var billAllocs = rows
                            .Where(r => r.bill_public_id != null)
                            .Select(r => new MaintenancePaymentAllocation((Guid)r.bill_public_id, (decimal)r.allocated_amount, (string?)r.period))
                            .ToList();

                        // Advance = rows where BOTH bill_id and adjustment_id are NULL.
                        var advance = rows
                            .Where(r => r.bill_id == null && r.adjustment_id == null)
                            .Sum(r => (decimal)r.allocated_amount);

                        return new MaintenancePaymentResponse
                        {
                            FlatPublicId    = request.FlatPublicId,
                            Amount          = request.Amount,
                            PaymentDate     = request.PaymentDate,
                            ReferenceNumber = request.ReferenceNumber,
                            Notes           = request.Notes,
                            TotalPaid       = billAllocs.Sum(a => a.AllocatedAmount),
                            Allocations     = billAllocs,
                            RemainingAdvance = advance
                        };
                    }

                    // ── Step 2: Resolve payment mode (EF; outside lock scope) ─────────────
                    var paymentModeId = await _db.payment_modes
                        .Where(pm => pm.code == request.PaymentModeCode)
                        .Select(pm => pm.id)
                        .FirstOrDefaultAsync();
                    if (paymentModeId == 0)
                        throw new ValidationException($"Invalid payment mode code: {request.PaymentModeCode}");

                    var now = DateTime.UtcNow;

                    // ── Step 3: Lock flat row (serialises all concurrent payments for this flat) ──
                    var flat = await _dapper.QueryFirstOrDefaultAsync<FlatRow>(
                        conn, tx,
                        SqlQueries.LockFlatByPublicId,
                        new { FlatPublicId = request.FlatPublicId, SocietyId = societyId })
                        ?? throw new NotFoundException("Flat not found or does not belong to society.");

                    var remaining  = request.Amount;
                    var allocations = new List<MaintenancePaymentAllocation>();

                    // ── Step 4: Allocate to OpeningBalance adjustments (FIFO, oldest first) ──
                    var adjustments = (await _dapper.QueryAsync<AdjustmentRow>(
                        conn, tx,
                        SqlQueries.LockOpeningBalanceAdjustments,
                        new { FlatId = flat.id, SocietyId = societyId, EntryType = EntryTypeCodes.OpeningBalance })).ToList();

                    foreach (var adj in adjustments)
                    {
                        if (remaining <= 0) break;

                        var allocation = Math.Min(adj.remaining_amount, remaining);
                        if (allocation <= 0) continue;

                        await _dapper.ExecuteAsync(
                            conn, tx,
                            SqlQueries.DeductAdjustmentRemainingAmount,
                            new { AdjustmentId = adj.id, Allocation = allocation, SocietyId = societyId });

                        await _dapper.ExecuteAsync(
                            conn, tx,
                            SqlQueries.InsertMaintenancePayment,
                            new
                            {
                                SocietyId       = societyId,
                                FlatId          = flat.id,
                                BillId          = (long?)null,
                                AdjustmentId    = adj.id,
                                Amount          = allocation,
                                PaymentDate     = request.PaymentDate,
                                PaymentModeId   = paymentModeId,
                                ReferenceNumber = request.ReferenceNumber,
                                ReceiptUrl      = request.ReceiptUrl,
                                Notes           = "Opening Balance Clearance",
                                RecordedBy      = userId,
                                IdempotencyKey  = idempotencyKey,
                                Now             = now
                            });

                        remaining -= allocation;
                    }

                    // ── Step 5: Lock unpaid bills FIFO (oldest period first) ──────────────
                    var bills = (await _dapper.QueryAsync<BillRow>(
                        conn, tx,
                        SqlQueries.LockUnpaidBillsByFlat,
                        new { FlatId = flat.id, SocietyId = societyId })).ToList();

                    // ── Step 6: Allocate remaining amount to bills FIFO ───────────────────
                    foreach (var bill in bills)
                    {
                        if (remaining <= 0) break;

                        var balance    = bill.amount - bill.PaidAmount;
                        var allocation = Math.Min(balance, remaining);
                        if (allocation <= 0) continue;

                        await _dapper.ExecuteAsync(
                            conn, tx,
                            SqlQueries.InsertMaintenancePayment,
                            new
                            {
                                SocietyId       = societyId,
                                FlatId          = flat.id,
                                BillId          = bill.id,
                                AdjustmentId    = (long?)null,
                                Amount          = allocation,
                                PaymentDate     = request.PaymentDate,
                                PaymentModeId   = paymentModeId,
                                ReferenceNumber = request.ReferenceNumber,
                                ReceiptUrl      = request.ReceiptUrl,
                                Notes           = request.Notes,
                                RecordedBy      = userId,
                                IdempotencyKey  = idempotencyKey,
                                Now             = now
                            });

                        var newPaid = bill.PaidAmount + allocation;
                        await _dapper.ExecuteAsync(
                            conn, tx,
                            SqlQueries.UpdateBillPayment,
                            new
                            {
                                PaidAmount = newPaid,
                                StatusCode = newPaid >= bill.amount ? BillStatusCodes.Paid : BillStatusCodes.Partial,
                                BillId     = bill.id,
                                Now        = now
                            });

                        allocations.Add(new MaintenancePaymentAllocation(bill.public_id, allocation, bill.period));
                        remaining -= allocation;
                    }

                    // ── Step 7: If remaining > 0, record as advance ───────────────────────
                    if (remaining > 0)
                    {
                        await _dapper.ExecuteAsync(
                            conn, tx,
                            SqlQueries.InsertMaintenancePayment,
                            new
                            {
                                SocietyId       = societyId,
                                FlatId          = flat.id,
                                BillId          = (long?)null,
                                AdjustmentId    = (long?)null,
                                Amount          = remaining,
                                PaymentDate     = request.PaymentDate,
                                PaymentModeId   = paymentModeId,
                                ReferenceNumber = request.ReferenceNumber,
                                ReceiptUrl      = request.ReceiptUrl,
                                Notes           = "Advance Payment",
                                RecordedBy      = userId,
                                IdempotencyKey  = idempotencyKey,
                                Now             = now
                            });
                    }

                    // ── Step 8: Commit ────────────────────────────────────────────────────
                    await tx.CommitAsync();

                    return new MaintenancePaymentResponse
                    {
                        FlatPublicId     = flat.public_id,
                        Amount           = request.Amount,
                        PaymentDate      = request.PaymentDate,
                        ReferenceNumber  = request.ReferenceNumber,
                        Notes            = request.Notes,
                        TotalPaid        = allocations.Sum(a => a.AllocatedAmount),
                        Allocations      = allocations,
                        RemainingAdvance = remaining
                    };
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.Error(ex, "Error processing maintenance payment");
                    throw;
                }
            }
        }

        // ------------------------------------------------------------------ //
        //  Remaining interface methods                                         //
        // ------------------------------------------------------------------ //

        public async Task<MaintenancePaymentResponse> AllocateMaintenancePaymentAsync(
            long userId, CreateMaintenancePaymentRequest request, string idempotencyKey)
        {
            var req = new MaintenancePaymentRequest(
                request.FlatPublicId,
                request.Amount,
                request.PaymentDate,
                request.PaymentModeCode,
                request.ReferenceNumber,
                request.ReceiptUrl,
                request.Notes,
                idempotencyKey);
            return await ProcessPaymentAsync(req, userId);
        }

        public async Task<MaintenancePaymentResponse> CreateMaintenancePaymentAsync(
            long userId, CreateMaintenancePaymentRequest request)
        {
            return await AllocateMaintenancePaymentAsync(userId, request, Guid.NewGuid().ToString());
        }

        public async Task<MaintenancePaymentResponse> GetMaintenancePaymentAsync(
            Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            var payment   = await _maintenancePaymentRepo.GetByPublicIdAsync(publicId, societyId)
                ?? throw new NotFoundException("Maintenance payment", publicId.ToString());

            return MapToResponse(payment);
        }

        public async Task<IEnumerable<MaintenancePaymentResponse>> GetMaintenancePaymentsBySocietyAsync(
            long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            var payments  = await _maintenancePaymentRepo.GetBySocietyIdAsync(societyId);
            return payments.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaintenancePaymentResponse>> GetMaintenancePaymentsByFlatAsync(
            Guid flatPublicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            var payments  = await _maintenancePaymentRepo.GetByFlatPublicIdAsync(flatPublicId);
            return payments.Where(p => p.SocietyId == societyId).Select(MapToResponse);
        }

        public async Task<MaintenancePaymentResponse> UpdateMaintenancePaymentAsync(
            Guid publicId, long userId, UpdateMaintenancePaymentRequest request)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var payment = await _db.maintenance_payments
                .FirstOrDefaultAsync(p => p.public_id  == publicId
                                       && p.society_id == societyId
                                       && !p.is_deleted)
                ?? throw new NotFoundException("Maintenance payment", publicId.ToString());

            if (request.Amount.HasValue)         payment.amount           = request.Amount.Value;

            if (request.PaymentDate.HasValue)
            {
                var onboardingDate = await GetOnboardingDateAsync(societyId);
                var paymentDateOnly = DateOnly.FromDateTime(request.PaymentDate.Value);
                if (paymentDateOnly < onboardingDate)
                    throw new ValidationException(
                        $"Payment date ({paymentDateOnly:yyyy-MM-dd}) cannot be earlier than " +
                        $"the society onboarding date ({onboardingDate:yyyy-MM-dd}).");

                payment.payment_date = request.PaymentDate.Value;
            }
            if (request.ReferenceNumber != null) payment.reference_number = request.ReferenceNumber;
            if (request.ReceiptUrl != null)      payment.receipt_url      = request.ReceiptUrl;
            if (request.Notes != null)           payment.notes            = request.Notes;

            if (request.PaymentModeCode != null)
            {
                var mode = await _db.payment_modes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pm => pm.code == request.PaymentModeCode)
                    ?? throw new NotFoundException("Payment mode", request.PaymentModeCode);
                payment.payment_mode_id = mode.id;
            }

            await _db.SaveChangesAsync();

            var updated = await _maintenancePaymentRepo.GetByPublicIdAsync(publicId, societyId)
                ?? throw new InvalidOperationException("Failed to reload updated payment.");
            return MapToResponse(updated);
        }

        public async Task DeleteMaintenancePaymentAsync(Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            var payment = await _maintenancePaymentRepo.GetByPublicIdAsync(publicId, societyId)
                ?? throw new NotFoundException("Maintenance payment", publicId.ToString());

            await _maintenancePaymentRepo.DeleteByPublicIdAsync(publicId, societyId);
        }

        public async Task<IEnumerable<PaymentModeResponse>> GetPaymentModesAsync()
        {
            var modes = await _paymentModeRepo.GetAllAsync();
            return modes.Select(m => new PaymentModeResponse(m.Code, m.DisplayName));
        }

        public async Task<MaintenanceSummaryResponse> GetMaintenanceSummaryAsync(
            long societyId,
            string period)
        {
            if (string.IsNullOrEmpty(period) ||
                !Regex.IsMatch(period, @"^\d{4}-\d{2}$"))
                throw new ValidationException("Period format must be yyyy-MM (e.g., 2026-02)");

            var param = new { SocietyId = societyId, Period = period };
            var obParam = new { SocietyId = societyId, EntryType = EntryTypeCodes.OpeningBalance };

            // Run all four summary queries independently — simple, index-friendly, no subqueries.
            var totalCharges  = await _dapper.QueryFirstOrDefaultAsync<decimal>(SqlQueries.SummaryTotalCharges,  param);
            var totalCollected = await _dapper.QueryFirstOrDefaultAsync<decimal>(SqlQueries.SummaryTotalCollected, param);
            var billOutstanding = await _dapper.QueryFirstOrDefaultAsync<decimal>(SqlQueries.SummaryBillOutstanding, param);
            var obRemaining    = await _dapper.QueryFirstOrDefaultAsync<decimal>(SqlQueries.SummaryOpeningBalanceRemaining, obParam);

            var totalOutstanding      = billOutstanding + obRemaining;
            var collectionPercentage  = totalCharges > 0
                ? Math.Round(totalCollected / totalCharges * 100, 2)
                : 0m;

            return new MaintenanceSummaryResponse(
                TotalCharges:            totalCharges,
                TotalCollected:          totalCollected,
                BillOutstanding:         billOutstanding,
                OpeningBalanceRemaining: obRemaining,
                TotalOutstanding:        totalOutstanding,
                CollectionPercentage:    collectionPercentage);
        }


        // ------------------------------------------------------------------ //
        //  Private helpers                                                     //
        // ------------------------------------------------------------------ //

        private static MaintenancePaymentResponse MapToResponse(MaintenancePaymentEntity p) => new()
        {
            PublicId        = p.PublicId,
            SocietyPublicId = p.SocietyPublicId,
            FlatPublicId    = p.FlatPublicId,
            FlatNumber      = p.FlatNumber,
            Amount          = p.Amount,
            PaymentDate     = p.PaymentDate,
            PaymentModeName = p.PaymentModeName ?? string.Empty,
            ReferenceNumber = p.ReferenceNumber,
            ReceiptUrl      = p.ReceiptUrl,
            Notes           = p.Notes,
            RecordedByName  = p.RecordedByName,
            CreatedAt       = p.CreatedAt,
            Allocations     = p.BillPublicId.HasValue
                ? [new MaintenancePaymentAllocation(p.BillPublicId.Value, p.Amount, p.Period)]
                : []
        };

        /// <summary>
        /// Returns the society's onboarding date. Used to reject payment dates that
        /// fall before the society's financial epoch.
        /// </summary>
        private async Task<DateOnly> GetOnboardingDateAsync(long societyId)
        {
            var date = await _db.societies
                .AsNoTracking()
                .Where(s => s.id == societyId && !s.is_deleted)
                .Select(s => (DateOnly?)s.onboarding_date)
                .FirstOrDefaultAsync();

            if (date is null)
                throw new NotFoundException("Society", societyId.ToString());

            return date.Value;
        }

        // ── Dapper-only projection types (not tracked by EF Core) ──────────

        private sealed record FlatRow(long id, Guid public_id, string flat_no, long society_id);

        private sealed class AdjustmentRow
        {
            public long    id               { get; init; }
            public Guid    public_id        { get; init; }
            public decimal remaining_amount { get; init; }
        }

        private sealed class BillRow
        {
            public long    id          { get; init; }
            public Guid    public_id   { get; init; }
            public decimal amount      { get; init; }
            public decimal PaidAmount  { get; set; }
            public string  status_code { get; init; } = string.Empty;
            public string  period      { get; init; } = string.Empty;
        }
    }
}
