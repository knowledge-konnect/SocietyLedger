using System.ComponentModel.DataAnnotations;

namespace SocietyLedger.Application.DTOs.Billing
{
    /// <summary>
    /// Request body for the manual admin endpoint that triggers
    /// <see cref="IBillingService.GenerateMonthlyBillsAsync"/>.
    /// </summary>
    public record GenerateMonthlyBillsRequest
    {
        /// <summary>
        /// Target billing month in <c>YYYY-MM</c> format.
        /// Example: <c>"2026-03"</c>
        /// When omitted, defaults to the current UTC month.
        /// </summary>
        public string? BillingMonth { get; init; }

        /// <summary>
        /// Parses <see cref="BillingMonth"/> and returns the first day of that month as a
        /// <see cref="DateTime"/>.  Falls back to the first day of the current UTC month.
        /// </summary>
        public DateTime GetBillingMonthDate()
        {
            if (!string.IsNullOrWhiteSpace(BillingMonth) &&
                DateTime.TryParseExact(BillingMonth, "yyyy-MM",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsed))
            {
                return new DateTime(parsed.Year, parsed.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            var now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
