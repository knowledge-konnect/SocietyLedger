namespace SocietyLedger.Application.DTOs.Razorpay
{
    public class OrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public DateTime CreatedAt { get; set; }
    }
}
