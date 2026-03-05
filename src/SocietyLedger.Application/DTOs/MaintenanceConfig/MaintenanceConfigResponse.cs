namespace SocietyLedger.Application.DTOs.MaintenanceConfig
{
    /// <summary>
    /// Response DTO for maintenance billing configuration.
    /// </summary>
    public class MaintenanceConfigResponse
    {
        public Guid SocietyPublicId { get; set; }
        public decimal DefaultMonthlyCharge { get; set; }
        public int DueDayOfMonth { get; set; }
        public decimal LateFeePerMonth { get; set; }
        public int GracePeriodDays { get; set; }
    }
}
