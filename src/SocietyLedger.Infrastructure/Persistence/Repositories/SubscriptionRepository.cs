using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _db;

        public SubscriptionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Subscription?> GetByUserIdAsync(long userId)
        {
            var efSubscription = await _db.subscriptions
                .Include(s => s.plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.user_id == userId);

            return efSubscription?.ToDomain();
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            var efSubscription = await _db.subscriptions
                .Include(s => s.plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.id == id);

            return efSubscription?.ToDomain();
        }

        public async Task CreateAsync(Subscription subscription)
        {
            var efSubscription = subscription.ToEntity();
            efSubscription.created_at = DateTime.UtcNow;
            efSubscription.updated_at = DateTime.UtcNow;
            _db.subscriptions.Add(efSubscription);
            await _db.SaveChangesAsync();
            subscription.Id = efSubscription.id;
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            var efSubscription = subscription.ToEntity();
            efSubscription.updated_at = DateTime.UtcNow;
            _db.subscriptions.Update(efSubscription);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subscription>> GetExpiredTrialsAsync()
        {
            var now = DateTime.UtcNow;
            var efSubscriptions = await _db.subscriptions
                .Include(s => s.plan)
                .Where(s => s.status == SubscriptionStatusCodes.Trial && s.trial_end < now)
                .ToListAsync();

            return efSubscriptions.Select(s => s.ToDomain());
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
        {
            var efSubscriptions = await _db.subscriptions
                .Include(s => s.plan)
                .Where(s => s.status == SubscriptionStatusCodes.Active)
                .ToListAsync();

            return efSubscriptions.Select(s => s.ToDomain());
        }
    }
}