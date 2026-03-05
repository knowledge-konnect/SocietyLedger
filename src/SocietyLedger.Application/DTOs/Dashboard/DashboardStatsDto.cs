using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// Aggregate financial snapshot for the selected period.
    /// Maps to the "snapshot" key in get_dashboard_all_data_json().
    /// </summary>
    public class SnapshotInfo
    {
        [JsonPropertyName("total_flats")]
        public int TotalFlats { get; set; }

        [JsonPropertyName("bank_balance")]
        public decimal BankBalance { get; set; }

        [JsonPropertyName("total_billed")]
        public decimal TotalBilled { get; set; }

        [JsonPropertyName("net_cash_flow")]
        public decimal NetCashFlow { get; set; }

        [JsonPropertyName("total_expense")]
        public decimal TotalExpense { get; set; }

        [JsonPropertyName("collection_rate")]
        public decimal CollectionRate { get; set; }

        [JsonPropertyName("total_collected")]
        public decimal TotalCollected { get; set; }

        [JsonPropertyName("bill_outstanding")]
        public decimal BillOutstanding { get; set; }

        [JsonPropertyName("opening_dues_remaining")]
        public decimal OpeningDuesRemaining { get; set; }

        [JsonPropertyName("total_member_outstanding")]
        public decimal TotalMemberOutstanding { get; set; }
    }
}
