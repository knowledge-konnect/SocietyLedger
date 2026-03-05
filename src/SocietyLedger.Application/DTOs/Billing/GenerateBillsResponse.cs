namespace SocietyLedger.Application.DTOs.Billing
{
    public record GenerateBillsResponse(
        string Period,
        int BillsCreated
    );
}
