using SocietyLedger.Application.DTOs.Subscription;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface ISubscriptionService
    {
        Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(long userId);
        Task<SubscribeResponse> SubscribeAsync(long userId, SubscribeRequest request);
        Task CancelSubscriptionAsync(long userId, CancelSubscriptionRequest request);
        Task CreateTrialSubscriptionAsync(long userId);
    }
}