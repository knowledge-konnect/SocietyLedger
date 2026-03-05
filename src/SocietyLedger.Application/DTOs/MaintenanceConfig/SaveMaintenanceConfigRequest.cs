namespace SocietyLedger.Application.DTOs.MaintenanceConfig
{
    /// <summary>
    /// Request DTO for creating or updating a society's maintenance billing configuration.
    /// </summary>
    public class SaveMaintenanceConfigRequest
    {
        /// <summary>Default monthly charge per flat. Must be >= 0.</summary>
        public decimal DefaultMonthlyCharge { get; set; }

        /// <summary>Day of month when maintenance is due (1–28).</summary>
        public int DueDayOfMonth { get; set; }

        /// <summary>Late fee charged per month after grace period. Must be >= 0.</summary>
        public decimal LateFeePerMonth { get; set; }

        /// <summary>Number of grace days before late fee applies. Must be >= 0.</summary>
        public int GracePeriodDays { get; set; }
    }
}
