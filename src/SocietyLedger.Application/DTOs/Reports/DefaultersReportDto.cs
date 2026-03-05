using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Reports
{
    public class DefaulterDto
    {
        [JsonPropertyName("flat_no")]
        public string FlatNo { get; set; } = null!;

        [JsonPropertyName("owner_name")]
        public string OwnerName { get; set; } = null!;

        [JsonPropertyName("contact_mobile")]
        public string? ContactMobile { get; set; }

        [JsonPropertyName("total_billed")]
        public decimal TotalBilled { get; set; }

        [JsonPropertyName("total_paid")]
        public decimal TotalPaid { get; set; }

        [JsonPropertyName("total_outstanding")]
        public decimal TotalOutstanding { get; set; }

        [JsonPropertyName("pending_months")]
        public int PendingMonths { get; set; }

        [JsonPropertyName("oldest_due_period")]
        public string? OldestDuePeriod { get; set; }

        [JsonPropertyName("latest_due_period")]
        public string? LatestDuePeriod { get; set; }
    }
}
