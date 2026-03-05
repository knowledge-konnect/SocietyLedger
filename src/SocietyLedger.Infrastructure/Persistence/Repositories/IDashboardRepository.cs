using SocietyLedger.Application.DTOs.Dashboard;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public interface IDashboardRepository
    {
        /// <summary>
        /// Gets complete dashboard data from PostgreSQL function
        /// </summary>
        Task<DashboardResponseDto> GetDashboardDataAsync(
            long societyId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default);
    }
}
