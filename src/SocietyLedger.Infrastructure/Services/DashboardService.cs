using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.Dashboard;
using SocietyLedger.Infrastructure.Persistence.Repositories;

namespace SocietyLedger.Infrastructure.Services
{
    public interface IDashboardService
    {
        /// <summary>
        /// Gets complete dashboard data for a society, with a short-lived in-memory cache.
        /// </summary>
        Task<DashboardResponseDto> GetDashboardDataAsync(
            long societyId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default);
    }

    public class DashboardService : IDashboardService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(3);

        private readonly IDashboardRepository _dashboardRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            IMemoryCache cache,
            ILogger<DashboardService> logger)
        {
            _dashboardRepository = dashboardRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DashboardResponseDto> GetDashboardDataAsync(
            long societyId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            if (societyId <= 0)
                throw new ArgumentException("Society ID must be greater than 0", nameof(societyId));

            var cacheKey = BuildCacheKey(societyId, startDate, endDate);

            if (_cache.TryGetValue(cacheKey, out DashboardResponseDto? cached) && cached is not null)
            {
                _logger.LogDebug(
                    "Dashboard cache HIT for societyId: {SocietyId}, key: {CacheKey}",
                    societyId, cacheKey);
                return cached;
            }

            try
            {
                var data = await _dashboardRepository.GetDashboardDataAsync(
                    societyId, startDate, endDate, cancellationToken);

                _cache.Set(cacheKey, data, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    // Evict immediately under memory pressure
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogInformation(
                    "Dashboard data fetched and cached for societyId: {SocietyId} (TTL: {TTL}s)",
                    societyId, CacheDuration.TotalSeconds);

                return data;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching dashboard data for societyId: {SocietyId}",
                    societyId);
                throw;
            }
        }

        /// <summary>
        /// Builds a deterministic cache key scoped to the society and optional date range.
        /// </summary>
        private static string BuildCacheKey(long societyId, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate.HasValue
                ? startDate.Value.Date.ToString("yyyy-MM-dd")
                : "null";

            var end = endDate.HasValue
                ? endDate.Value.Date.ToString("yyyy-MM-dd")
                : "null";

            return $"dashboard:{societyId}:{start}:{end}";
        }
    }
}
