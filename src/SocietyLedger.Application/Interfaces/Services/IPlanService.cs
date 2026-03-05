using SocietyLedger.Application.DTOs.Plan;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanResponse>> GetActivePlansAsync();
        Task<PlanResponse?> GetPlanByIdAsync(Guid id);
    }
}