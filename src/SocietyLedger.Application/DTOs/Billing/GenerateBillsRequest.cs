namespace SocietyLedger.Application.DTOs.Billing
{
    /// <summary>
    /// Request to manually generate monthly bills for all active flats.
    /// Period must be in YYYY-MM format (e.g. "2026-03").
    /// </summary>
    public record GenerateBillsRequest(string Period);
}
