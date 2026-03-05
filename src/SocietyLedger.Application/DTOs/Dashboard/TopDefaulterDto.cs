using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// A flat with an outstanding balance.
    /// Maps to each item in the "top_defaulters" array.
    /// </summary>
    public class TopDefaulter
    {
        [JsonPropertyName("flat_no")]
        public string FlatNo { get; set; } = string.Empty;

        [JsonPropertyName("outstanding")]
        public decimal Outstanding { get; set; }
    }
}
