using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface ISubscriptionEventRepository
    {
        Task CreateAsync(SubscriptionEvent subscriptionEvent);
        Task<IEnumerable<SubscriptionEvent>> GetByUserIdAsync(long userId);
    }
}