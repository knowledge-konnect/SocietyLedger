using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.Plan;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;

namespace SocietyLedger.Infrastructure.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly ILogger<PlanService> _logger;

        public PlanService(IPlanRepository planRepository, ILogger<PlanService> logger)
        {
            _planRepository = planRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<PlanResponse>> GetActivePlansAsync()
        {
            var plans = await _planRepository.GetActivePlansAsync();
            return plans.Select(p => new PlanResponse
            {
                Id = p.Id,
                Name = p.Name,
                MonthlyAmount = p.MonthlyAmount,
                Currency = p.Currency,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
        }

        public async Task<PlanResponse?> GetPlanByIdAsync(Guid id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null)
                throw new NotFoundException("Plan", id.ToString());

            return new PlanResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                MonthlyAmount = plan.MonthlyAmount,
                Currency = plan.Currency,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt
            };
        }
    }
}