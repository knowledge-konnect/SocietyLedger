using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using SocietyLedger.Application.DTOs.Billing;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;
using SocietyLedger.Infrastructure.Services.Common;
using System.Diagnostics;

namespace SocietyLedger.Infrastructure.Services
{
    public sealed class BillingService : IBillingService
    {
        private readonly AppDbContext _db;
        private readonly IUserContext _userContext;
        private readonly ILogger<BillingService> _logger;

        public BillingService(AppDbContext db, IUserContext userContext, ILogger<BillingService> logger)
        {
            _db          = db;
            _userContext = userContext;
            _logger      = logger;
        }

        // ------------------------------------------------------------------ //
        //  Generate bills manually for a given period                          //
        // ------------------------------------------------------------------ //

        public async Task<GenerateBillsResponse> GenerateBillsAsync(long userId, string period)
        {
            var (user, societyId) = await _userContext.GetUserContextAsync(userId);

            // Guard: duplicate generation — honour the unique constraint early
            var alreadyExists = await _db.bills
                .AnyAsync(b => b.society_id == societyId
                            && b.period     == period
                            && !b.is_deleted);

            if (alreadyExists)
                throw new ConflictException(
                    $"Bills for period '{period}' have already been generated for this society.");

            // Fetch the society's maintenance config for the default monthly charge fallback.
            var maintConfig = await _db.maintenance_configs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.society_id == societyId);

            // Fetch all active flats for this society.
            var flats = await _db.flats
                .AsNoTracking()
                .Where(f => f.society_id == societyId && !f.is_deleted)
                .Select(f => new { f.id, f.maintenance_amount })
                .ToListAsync();

            if (flats.Count == 0)
                throw new NotFoundException("Flats", $"No active flats found for society {societyId}.");

            var now = DateTime.UtcNow;

            // Build bill entities for bulk insert.
            // Amount priority (same rule used by GenerateMonthlyBillsAsync):
            //   1. flat.maintenance_amount  — flat-level override when configured (> 0)
            //   2. maintenance_config.default_monthly_charge — society-wide default
            //   3. 0 if neither is configured (edge-case guard)
            var bills = flats.Select(f => new bill
            {
                society_id   = societyId,
                flat_id      = f.id,
                period       = period,
                amount       = f.maintenance_amount > 0
                                   ? f.maintenance_amount
                                   : (maintConfig?.default_monthly_charge ?? 0m),
                status_code  = BillStatusCodes.Unpaid,
                generated_by = userId,
                generated_at = now,
                created_at   = now,
                is_deleted   = false,
                source       = "manual"
            }).ToList();

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                await _db.bills.AddRangeAsync(bills);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            Log.Information(
                "Bills generated: societyId={SocietyId}, period={Period}, count={Count}, by={UserId}",
                societyId, period, bills.Count, userId);

            return new GenerateBillsResponse(period, bills.Count);
        }

        // ------------------------------------------------------------------ //
        //  Billing status check for the current calendar month                //
        // ------------------------------------------------------------------ //

        public async Task<BillingStatusResponse> GetBillingStatusAsync(long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");

            var count = await _db.bills
                .CountAsync(b => b.society_id == societyId
                              && b.period     == currentMonth
                              && !b.is_deleted);

            return new BillingStatusResponse(
                CurrentMonth   : currentMonth,
                IsGenerated    : count > 0,
                GeneratedCount : count
            );
        }

        // ------------------------------------------------------------------ //
        //  Automated monthly bill generation — called by Hangfire job AND     //
        //  the manual admin trigger endpoint.                                  //
        //  All business logic lives here; callers only orchestrate.           //
        // ------------------------------------------------------------------ //

        public async Task<BillingResult> GenerateMonthlyBillsAsync(DateTime billingMonth)
        {
            var stopwatch = Stopwatch.StartNew();
            var period    = billingMonth.ToString("yyyy-MM");
            int totalFlatsProcessed = 0;
            int billsCreated        = 0;
            int billsSkipped        = 0;
            int failedSocieties     = 0;

            _logger.LogInformation(
                "GenerateMonthlyBillsAsync started. BillingMonth={BillingMonth:yyyy-MM}",
                billingMonth);

            // ---- 1. Collect all distinct society IDs that have active flats ----------
            var societyIds = await _db.flats
                .Where(f => !f.is_deleted)
                .Select(f => f.society_id)
                .Distinct()
                .ToListAsync();

            if (societyIds.Count == 0)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "GenerateMonthlyBillsAsync: no active societies/flats found. Period={Period}",
                    period);

                return new BillingResult
                {
                    TotalFlatsProcessed = 0,
                    BillsCreated        = 0,
                    BillsSkipped        = 0,
                    ExecutionTime       = stopwatch.Elapsed,
                    Success             = true,
                    ErrorMessage        = "No active societies with flats found."
                };
            }

            // ---- 2. Process each society independently ----------------------------
            foreach (var societyId in societyIds)
            {
                try
                {
                    // Fetch the society's maintenance config to get the default monthly charge.
                    var maintConfig = await _db.maintenance_configs
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.society_id == societyId);

                    // Project to the minimum fields needed — avoid loading full flat entities.
                    var flats = await _db.flats
                        .AsNoTracking()
                        .Where(f => f.society_id == societyId && !f.is_deleted)
                        .Select(f => new { f.id, f.maintenance_amount })
                        .ToListAsync();

                    totalFlatsProcessed += flats.Count;

                    if (flats.Count == 0)
                    {
                        _logger.LogWarning(
                            "Society {SocietyId}: no active flats found, skipping.",
                            societyId);
                        continue;
                    }

                    // ---- Bulk idempotency check: one query per society, not per flat ----
                    // Fetch the set of flat IDs that already have a bill for this period.
                    var flatIdsWithBill = await _db.bills
                        .Where(b => b.society_id == societyId
                                 && b.period     == period
                                 && !b.is_deleted)
                        .Select(b => b.flat_id)
                        .ToHashSetAsync();

                    var now      = DateTime.UtcNow;
                    var newBills = new List<bill>(flats.Count);

                    foreach (var flat in flats)
                    {
                        if (flatIdsWithBill.Contains(flat.id))
                        {
                            billsSkipped++;
                            continue;
                        }

                        // Amount priority:
                        //   1. flat.maintenance_amount  (flat-level override, set when flat is configured)
                        //   2. maintenance_config.default_monthly_charge  (society-wide default)
                        //   3. 0 if neither is configured (edge-case guard)
                        var amount = flat.maintenance_amount > 0
                            ? flat.maintenance_amount
                            : (maintConfig?.default_monthly_charge ?? 0m);

                        newBills.Add(new bill
                        {
                            society_id   = societyId,
                            flat_id      = flat.id,
                            period       = period,
                            amount       = amount,
                            status_code  = BillStatusCodes.Unpaid,
                            generated_by = null,          // system-generated; no user context
                            generated_at = now,
                            created_at   = now,
                            is_deleted   = false,
                            source       = "scheduled"    // distinguishes automated from manual
                        });
                    }

                    // ---- Persist with a per-society transaction ----------------------
                    if (newBills.Count > 0)
                    {
                        await using var tx = await _db.Database.BeginTransactionAsync();
                        try
                        {
                            await _db.bills.AddRangeAsync(newBills);
                            await _db.SaveChangesAsync();
                            await tx.CommitAsync();
                            billsCreated += newBills.Count;
                        }
                        catch
                        {
                            await tx.RollbackAsync();
                            throw;
                        }
                    }

                    _logger.LogInformation(
                        "Society {SocietyId}: created={BillsCreated}, skipped={BillsSkipped}, period={Period}.",
                        societyId, newBills.Count, flatIdsWithBill.Count == 0 ? 0 : flats.Count - newBills.Count, period);
                }
                catch (Exception ex)
                {
                    // Log the failure for this society but continue processing others.
                    // A failure in one society must NOT prevent billing for the remaining ones.
                    _logger.LogError(
                        ex,
                        "Society {SocietyId}: billing failed for period {Period}. Error: {Error}",
                        societyId, period, ex.Message);

                    failedSocieties++;
                }
            }

            stopwatch.Stop();

            var hasFailures = failedSocieties > 0;

            if (hasFailures)
                _logger.LogWarning(
                    "GenerateMonthlyBillsAsync completed with partial failures. Period={Period}, " +
                    "TotalFlatsProcessed={TotalFlatsProcessed}, BillsCreated={BillsCreated}, " +
                    "BillsSkipped={BillsSkipped}, FailedSocieties={FailedSocieties}, Duration={Duration:c}",
                    period, totalFlatsProcessed, billsCreated, billsSkipped, failedSocieties, stopwatch.Elapsed);
            else
                _logger.LogInformation(
                    "GenerateMonthlyBillsAsync completed. Period={Period}, " +
                    "TotalFlatsProcessed={TotalFlatsProcessed}, BillsCreated={BillsCreated}, " +
                    "BillsSkipped={BillsSkipped}, Duration={Duration:c}",
                    period, totalFlatsProcessed, billsCreated, billsSkipped, stopwatch.Elapsed);

            return new BillingResult
            {
                TotalFlatsProcessed = totalFlatsProcessed,
                BillsCreated        = billsCreated,
                BillsSkipped        = billsSkipped,
                FailedSocieties     = failedSocieties,
                ExecutionTime       = stopwatch.Elapsed,
                Success             = !hasFailures,
                ErrorMessage        = hasFailures
                    ? $"{failedSocieties} society(s) failed to process for period {period}. Check logs for details."
                    : null
            };
        }

    }
}
