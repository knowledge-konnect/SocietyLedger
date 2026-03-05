using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly AppDbContext _db;

        public PlanRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Plan?> GetByIdAsync(Guid id)
        {
            var efPlan = await _db.plans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == id);

            return efPlan?.ToDomain();
        }

        public async Task<IEnumerable<Plan>> GetActivePlansAsync()
        {
            var efPlans = await _db.plans
                .Where(p => p.is_active == true)
                .AsNoTracking()
                .ToListAsync();

            return efPlans.Select(p => p.ToDomain());
        }
    }
}