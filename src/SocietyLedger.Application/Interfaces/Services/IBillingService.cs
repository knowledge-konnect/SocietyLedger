using SocietyLedger.Application.DTOs.Billing;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IBillingService
    {
        /// <summary>
        /// Generates one bill per active flat for the given period.
        /// Throws ConflictException if bills already exist for that society + period.
        /// Throws NotFoundException if society has no active flats.
        /// Used by the manual per-society endpoint.
        /// </summary>
        Task<GenerateBillsResponse> GenerateBillsAsync(long userId, string period);

        /// <summary>
        /// Returns whether bills have already been generated for the current calendar month.
        /// </summary>
        Task<BillingStatusResponse> GetBillingStatusAsync(long userId);

        /// <summary>
        /// Generates monthly maintenance bills for ALL active societies in the platform.
        /// Intended to be called by the Hangfire recurring job (1st of every month at 00:05)
        /// AND by the manual admin trigger endpoint. Contains all business logic; the caller
        /// is only responsible for orchestration and logging.
        ///
        /// Billing rules applied per society:
        ///   - Uses the society's active <c>maintenance_plan</c> for amount calculation.
        ///   - Supports <c>fixed</c> (flat-rate) and <c>per_sqft</c> billing types.
        ///   - Falls back to <c>flat.maintenance_amount</c> when no active plan is found.
        ///   - Skips any flat that already has a bill for <paramref name="billingMonth"/> (idempotency).
        ///   - Wraps each society's bill inserts in a database transaction for consistency.
        /// </summary>
        /// <param name="billingMonth">
        ///   The target month; only year and month components are used
        ///   (e.g. <c>new DateTime(2026, 3, 1)</c> → period "2026-03").
        /// </param>
        /// <returns>
        ///   A <see cref="BillingResult"/> describing how many bills were created, skipped,
        ///   and total execution time. Exceptions propagate to the caller; Hangfire will
        ///   retry automatically on failure.
        /// </returns>
        Task<BillingResult> GenerateMonthlyBillsAsync(DateTime billingMonth);
    }
}
