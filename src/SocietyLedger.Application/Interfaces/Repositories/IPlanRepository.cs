using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IPlanRepository
    {
        Task<Plan?> GetByIdAsync(Guid id);
        Task<IEnumerable<Plan>> GetActivePlansAsync();
    }
}