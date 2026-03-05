namespace SocietyLedger.Application.DTOs.Invoice
{
    public class PayInvoiceRequest
    {
        public string PaymentMethod { get; set; } = null!;
        public string? PaymentReference { get; set; }
        public decimal? Amount { get; set; } // Optional, can use invoice amount
    }
}