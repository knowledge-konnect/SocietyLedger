namespace SocietyLedger.Application.DTOs.Razorpay
{
    public class PaymentVerificationResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime VerifiedAt { get; set; }
        public bool IsVerified { get; set; }
    }
}
