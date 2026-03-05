namespace SocietyLedger.Application.DTOs.Invoice
{
    public class InvoiceResponse
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string InvoiceType { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public string Status { get; set; } = null!;
        public DateOnly? PeriodStart { get; set; }
        public DateOnly? PeriodEnd { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentReference { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}