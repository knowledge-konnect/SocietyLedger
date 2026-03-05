using Hangfire;
using Microsoft.Extensions.Logging;
using SocietyLedger.Application.Interfaces.Services;

namespace SocietyLedger.Infrastructure.Jobs
{
    /// <summary>
    /// Hangfire recurring job that triggers automated monthly maintenance bill generation.
    ///
    /// Scheduling:  Cron "5 0 1 * *"  — 1st of every month at 00:05 server time (UTC).
    /// Retries:     Up to 3 automatic attempts on failure (see <see cref="AutomaticRetryAttribute"/>).
    /// Concurrency: Only one instance may run at a time (see <see cref="DisableConcurrentExecutionAttribute"/>).
    ///
    /// ARCHITECTURE RULE — this class intentionally contains NO business logic.
    /// It only:
    ///   1. Determines the billing month (1st day of current UTC month).
    ///   2. Delegates to <see cref="IBillingService.GenerateMonthlyBillsAsync"/>.
    ///   3. Logs structured telemetry before and after execution.
    ///   4. Re-throws on unexpected exceptions so Hangfire can retry.
    /// </summary>
    [AutomaticRetry(Attempts = 3, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public sealed class MonthlyBillingJob
    {
        private readonly IBillingService _billingService;
        private readonly ILogger<MonthlyBillingJob> _logger;

        public MonthlyBillingJob(IBillingService billingService, ILogger<MonthlyBillingJob> logger)
        {
            _billingService = billingService;
            _logger         = logger;
        }

        /// <summary>
        /// Entry point invoked by Hangfire.
        /// Builds the billing month from the current UTC date and delegates all work to
        /// <see cref="IBillingService.GenerateMonthlyBillsAsync"/>.
        /// </summary>
        public async Task ExecuteAsync()
        {
            // Always bill for the 1st of the current UTC month.
            var billingMonth = new DateTime(
                DateTime.UtcNow.Year,
                DateTime.UtcNow.Month,
                1,
                0, 0, 0,
                DateTimeKind.Utc);

            _logger.LogInformation(
                "[MonthlyBillingJob] Job started. BillingMonth={BillingMonth:yyyy-MM}",
                billingMonth);

            try
            {
                var result = await _billingService.GenerateMonthlyBillsAsync(billingMonth);

                _logger.LogInformation(
                    "[MonthlyBillingJob] Completed. " +
                    "BillingMonth={BillingMonth:yyyy-MM}, " +
                    "TotalFlatsProcessed={TotalFlatsProcessed}, " +
                    "BillsCreated={BillsCreated}, " +
                    "BillsSkipped={BillsSkipped}, " +
                    "ExecutionTime={ExecutionTime:c}, " +
                    "Success={Success}",
                    billingMonth,
                    result.TotalFlatsProcessed,
                    result.BillsCreated,
                    result.BillsSkipped,
                    result.ExecutionTime,
                    result.Success);

                // Surface business-level failures as job failures so Hangfire retries them.
                if (!result.Success)
                {
                    _logger.LogError(
                        "[MonthlyBillingJob] Service reported failure. ErrorMessage={ErrorMessage}",
                        result.ErrorMessage);

                    throw new InvalidOperationException(
                        $"Monthly billing failed for period {billingMonth:yyyy-MM}: {result.ErrorMessage}");
                }
            }
            catch (InvalidOperationException)
            {
                // Already logged above; let Hangfire see the exception for retry.
                throw;
            }
            catch (Exception ex)
            {
                // Unexpected (e.g. DB outage, network error) — log full details and re-throw.
                _logger.LogError(
                    ex,
                    "[MonthlyBillingJob] Unexpected error during billing for {BillingMonth:yyyy-MM}.",
                    billingMonth);

                throw;   // Hangfire will retry according to AutomaticRetry policy above.
            }
        }
    }
}
