using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// Period window returned by the dashboard function.
    /// </summary>
    public class PeriodInfo
    {
        [JsonPropertyName("start")]
        public DateTime Start { get; set; }

        [JsonPropertyName("end")]
        public DateTime End { get; set; }

        [JsonPropertyName("period_key")]
        public string PeriodKey { get; set; } = string.Empty;
    }
}
