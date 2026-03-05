using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Reports
{
    public class CollectionSummaryDto
    {
        [JsonPropertyName("total_billed")]
        public decimal TotalBilled { get; set; }

        [JsonPropertyName("total_collected")]
        public decimal TotalCollected { get; set; }

        [JsonPropertyName("total_outstanding")]
        public decimal TotalOutstanding { get; set; }

        [JsonPropertyName("total_flats")]
        public int TotalFlats { get; set; }

        [JsonPropertyName("periods")]
        public List<CollectionPeriodDto> Periods { get; set; } = new();
    }

    public class CollectionPeriodDto
    {
        [JsonPropertyName("period")]
        public string Period { get; set; } = null!;

        [JsonPropertyName("total_billed")]
        public decimal TotalBilled { get; set; }

        [JsonPropertyName("total_collected")]
        public decimal TotalCollected { get; set; }

        [JsonPropertyName("total_outstanding")]
        public decimal TotalOutstanding { get; set; }

        [JsonPropertyName("flats_billed")]
        public int FlatsBilled { get; set; }

        [JsonPropertyName("flats_paid")]
        public int FlatsPaid { get; set; }

        [JsonPropertyName("flats_partial")]
        public int FlatsPartial { get; set; }

        [JsonPropertyName("flats_unpaid")]
        public int FlatsUnpaid { get; set; }
    }
}
