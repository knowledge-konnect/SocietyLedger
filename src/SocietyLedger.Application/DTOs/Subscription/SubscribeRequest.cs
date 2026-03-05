namespace SocietyLedger.Application.DTOs.Subscription
{
    public class SubscribeRequest
    {
        public Guid PlanId { get; set; }
        public string PaymentMethod { get; set; } = null!; // "razorpay", "bank_transfer", etc.
        public string? PaymentReference { get; set; } // For offline payments
        public decimal? Amount { get; set; } // Optional, can be fetched from plan
    }
}