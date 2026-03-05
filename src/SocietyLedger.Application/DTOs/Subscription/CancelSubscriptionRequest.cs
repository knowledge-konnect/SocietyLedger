namespace SocietyLedger.Application.DTOs.Subscription
{
    public class CancelSubscriptionRequest
    {
        public string? Reason { get; set; }
        public bool CancelImmediately { get; set; } = false; // If true, cancel now, else at period end
    }
}