namespace SocietyLedger.Application.DTOs.Expense
{
    public class CreateExpenseRequest
    {
        public DateOnly Date { get; set; }
        public string CategoryCode { get; set; } = null!;
        public string? Vendor { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
    }
}
