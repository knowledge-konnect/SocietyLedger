using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Dashboard
{
    /// <summary>
    /// Expense category breakdown for the selected period.
    /// Maps to each item in the "expense_breakdown" array.
    /// </summary>
    public class ExpenseBreakdownItem
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }
    }
}
