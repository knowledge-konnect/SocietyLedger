using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// A single recent payment or expense event.
    /// Maps to each item in the "recent_activity" array.
    /// </summary>
    public class RecentActivityItem
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
