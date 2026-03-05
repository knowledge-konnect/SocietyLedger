using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// Top-level response from public.get_dashboard_all_data_json()
    /// </summary>
    public class DashboardResponseDto
    {
        [JsonPropertyName("period")]
        public PeriodInfo Period { get; set; } = new();

        [JsonPropertyName("snapshot")]
        public SnapshotInfo Snapshot { get; set; } = new();

        [JsonPropertyName("trends")]
        public List<MonthlyTrend> Trends { get; set; } = new();

        [JsonPropertyName("top_defaulters")]
        public List<TopDefaulter> TopDefaulters { get; set; } = new();

        [JsonPropertyName("recent_activity")]
        public List<RecentActivityItem> RecentActivity { get; set; } = new();

        [JsonPropertyName("expense_breakdown")]
        public List<ExpenseBreakdownItem> ExpenseBreakdown { get; set; } = new();
    }
}
