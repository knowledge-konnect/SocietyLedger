using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Reports
{
    public class PaymentRegisterDto
    {
        [JsonPropertyName("flat_no")]
        public string FlatNo { get; set; } = null!;

        [JsonPropertyName("owner_name")]
        public string OwnerName { get; set; } = null!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("payment_mode")]
        public string? PaymentMode { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("period")]
        public string? Period { get; set; }

        /// <summary>
        /// "Current" if period matches payment month, "Arrear" if clearing a past bill, null for advance.
        /// </summary>
        [JsonPropertyName("period_label")]
        public string? PeriodLabel { get; set; }

        [JsonPropertyName("recorded_by")]
        public string? RecordedBy { get; set; }
        
        [JsonPropertyName("date_paid")]
        public DateOnly DatePaid { get; set; }
    }
}

