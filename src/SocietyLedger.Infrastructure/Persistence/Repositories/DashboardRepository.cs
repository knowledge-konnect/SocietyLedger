using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using SocietyLedger.Application.DTOs.Dashboard;
using SocietyLedger.Infrastructure.Data;
using System.Text.Json;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<DashboardRepository> _logger;

        // Reuse a single options instance — PropertyNameCaseInsensitive handles all snake_case keys.
        private static readonly JsonSerializerOptions JsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        private const string DashboardSql =
            "SELECT public.get_dashboard_data(@SocietyId, @StartDate, @EndDate)";

        public DashboardRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<DashboardRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<DashboardResponseDto> GetDashboardDataAsync(
            long societyId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection() as NpgsqlConnection;
                await connection!.OpenAsync(cancellationToken);

                // Npgsql requires DateTimeKind.Unspecified for 'timestamp without time zone'.
                // Passing Local/Utc causes a runtime exception.
                var parameters = new DynamicParameters();
                parameters.Add("@SocietyId", societyId);
                parameters.Add("@StartDate", NormalizeTimestamp(startDate));
                parameters.Add("@EndDate", NormalizeTimestamp(endDate));

                var jsonResult = await connection.QueryFirstOrDefaultAsync<string>(
                    DashboardSql,
                    parameters,
                    commandTimeout: 30);

                if (string.IsNullOrWhiteSpace(jsonResult))
                {
                    _logger.LogWarning(
                        "Dashboard function returned no data for societyId: {SocietyId}",
                        societyId);
                    throw new InvalidOperationException(
                        $"No dashboard data found for society {societyId}.");
                }

                var data = JsonSerializer.Deserialize<DashboardResponseDto>(jsonResult, JsonOptions)
                    ?? throw new InvalidOperationException(
                        "Failed to deserialize dashboard response from database.");

                _logger.LogInformation(
                    "Dashboard data retrieved for societyId: {SocietyId}, startDate: {StartDate}, endDate: {EndDate}",
                    societyId, startDate, endDate);

                return data;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex,
                    "Error retrieving dashboard data for societyId: {SocietyId}",
                    societyId);
                throw;
            }
        }

        /// <summary>
        /// Strips timezone info from a DateTime so Npgsql can write it to
        /// 'timestamp without time zone' without throwing a DateTimeKind mismatch.
        /// </summary>
        private static DateTime? NormalizeTimestamp(DateTime? dt)
        {
            if (dt is null) return null;
            return DateTime.SpecifyKind(dt.Value.Date, DateTimeKind.Unspecified);
        }
    }
}
