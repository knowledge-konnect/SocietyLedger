namespace SocietyLedger.Application.DTOs.Subscription
{
    public class SubscribeResponse
    {
        public Guid SubscriptionId { get; set; }
        public Guid InvoiceId { get; set; }
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? PaymentUrl { get; set; } // For Razorpay integration
        public string? InvoiceNumber { get; set; }
    }
}