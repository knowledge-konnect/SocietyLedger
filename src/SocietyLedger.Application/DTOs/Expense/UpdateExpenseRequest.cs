namespace SocietyLedger.Application.DTOs.Expense
{
    public class UpdateExpenseRequest
    {
        public DateOnly? Date { get; set; }
        public string? CategoryCode { get; set; }
        public string? Vendor { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public long? AttachmentId { get; set; }
    }
}
