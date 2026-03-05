using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByUserIdAsync(long userId);
        Task<Subscription?> GetByIdAsync(Guid id);
        Task CreateAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task<IEnumerable<Subscription>> GetExpiredTrialsAsync();
        Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
    }
}