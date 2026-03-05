namespace SocietyLedger.Application.DTOs.Billing
{
    public record BillingStatusResponse(
        string CurrentMonth,
        bool IsGenerated,
        int GeneratedCount
    );
}
