using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Reports
{
    public class FundLedgerReportDto
    {
        [JsonPropertyName("opening_balance")]
        public decimal OpeningBalance { get; set; }

        [JsonPropertyName("total_opening_fund")]
        public decimal TotalOpeningFund { get; set; }

        [JsonPropertyName("total_collections")]
        public decimal TotalCollections { get; set; }

        [JsonPropertyName("total_expenses")]
        public decimal TotalExpenses { get; set; }

        [JsonPropertyName("closing_balance")]
        public decimal ClosingBalance { get; set; }

        [JsonPropertyName("entries")]
        public List<FundLedgerEntryDto> Entries { get; set; } = new();
    }

    public class FundLedgerEntryDto
    {
        /// <summary>
        /// The financial event date (transaction_date from society_fund_ledger).
        /// This is the authoritative date for all financial calculations and reports.
        /// The underlying PostgreSQL stored function must ORDER BY and filter on
        /// transaction_date — never on created_at.
        /// </summary>
        [JsonPropertyName("transaction_date")]
        public DateOnly TransactionDate { get; set; }

        /// <summary>
        /// Formatted date string for display purposes (ISO-8601: yyyy-MM-dd).
        /// Derived from <see cref="TransactionDate"/>.
        /// </summary>
        [JsonPropertyName("date")]
        public string Date => TransactionDate.ToString("yyyy-MM-dd");

        [JsonPropertyName("entry_type")]
        public string EntryType { get; set; } = null!;

        [JsonPropertyName("credit")]
        public decimal Credit { get; set; }

        [JsonPropertyName("debit")]
        public decimal Debit { get; set; }

        [JsonPropertyName("running_balance")]
        public decimal RunningBalance { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}

