using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class SubscriptionEventRepository : ISubscriptionEventRepository
    {
        private readonly AppDbContext _db;

        public SubscriptionEventRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(SubscriptionEvent subscriptionEvent)
        {
            var efEvent = subscriptionEvent.ToEntity();
            efEvent.created_at = DateTime.UtcNow;
            _db.subscription_events.Add(efEvent);
            await _db.SaveChangesAsync();
            subscriptionEvent.Id = efEvent.id;
        }

        public async Task<IEnumerable<SubscriptionEvent>> GetByUserIdAsync(long userId)
        {
            var efEvents = await _db.subscription_events
                .Where(se => se.user_id == userId)
                .OrderByDescending(se => se.created_at)
                .AsNoTracking()
                .ToListAsync();

            return efEvents.Select(e => e.ToDomain());
        }
    }
}