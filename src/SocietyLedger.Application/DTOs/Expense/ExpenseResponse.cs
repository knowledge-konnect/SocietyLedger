namespace SocietyLedger.Application.DTOs.Expense
{
    public class ExpenseResponse
    {
        public Guid PublicId { get; set; }
        public Guid SocietyPublicId { get; set; }
        public DateOnly DateIncurred { get; set; }
        public string CategoryCode { get; set; } = null!;
        public string? Vendor { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? ApprovedByName { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
