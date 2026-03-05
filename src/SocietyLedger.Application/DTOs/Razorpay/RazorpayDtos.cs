namespace SocietyLedger.Application.DTOs.Razorpay
{
    public class CreateOrderRequest
    {
        /// <summary>
        /// The plan the user is subscribing to. Amount is resolved server-side from this ID.
        /// </summary>
        public Guid PlanId { get; set; }
    }

    public class CreateOrderResponse
    {
        public string OrderId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string KeyId { get; set; } = null!;
    }

    public class VerifyPaymentRequest
    {
        public string OrderId { get; set; } = null!;
        public string PaymentId { get; set; } = null!;
        public string Signature { get; set; } = null!;
    }

    public class VerifyPaymentResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = null!;
    }

    public class WebhookPayload
    {
        public string Event { get; set; } = null!;
        public PaymentEntity Payment { get; set; } = null!;
    }

    public class PaymentEntity
    {
        public string Id { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}