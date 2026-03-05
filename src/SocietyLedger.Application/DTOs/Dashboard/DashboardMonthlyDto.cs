using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// Monthly income vs expense data point.
    /// Maps to each item in the "trends" array.
    /// </summary>
    public class MonthlyTrend
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("income")]
        public decimal Income { get; set; }

        [JsonPropertyName("expense")]
        public decimal Expense { get; set; }
    }
}
