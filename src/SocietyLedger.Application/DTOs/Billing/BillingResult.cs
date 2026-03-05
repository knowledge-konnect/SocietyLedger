namespace SocietyLedger.Application.DTOs.Billing
{
    /// <summary>
    /// Aggregated result returned by <see cref="IBillingService.GenerateMonthlyBillsAsync"/>.
    /// Used by both the Hangfire automated job and the manual admin endpoint.
    /// </summary>
    public sealed class BillingResult
    {
        /// <summary>Total number of active flats found across all processed societies.</summary>
        public int TotalFlatsProcessed { get; init; }

        /// <summary>Number of new bills successfully created.</summary>
        public int BillsCreated { get; init; }

        /// <summary>Number of flats skipped because a bill for the same period already existed (idempotency).</summary>
        public int BillsSkipped { get; init; }

        /// <summary>Number of societies that failed to process. Other societies are unaffected.</summary>
        public int FailedSocieties { get; init; }

        /// <summary>Wall-clock time spent processing.</summary>
        public TimeSpan ExecutionTime { get; init; }

        /// <summary>True when all societies completed without errors. False when one or more societies failed.</summary>
        public bool Success { get; init; }

        /// <summary>Populated with a summary when Success is false.</summary>
        public string? ErrorMessage { get; init; }
    }
}
