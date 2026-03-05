using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Reports
{
    public class IncomeVsExpenseDto
    {
        [JsonPropertyName("total_income")]
        public decimal TotalIncome { get; set; }

        [JsonPropertyName("total_expense")]
        public decimal TotalExpense { get; set; }

        [JsonPropertyName("net_balance")]
        public decimal NetBalance { get; set; }

        [JsonPropertyName("months")]
        public List<MonthlyIncomeExpenseDto> Months { get; set; } = new();
    }

    public class MonthlyIncomeExpenseDto
    {
        [JsonPropertyName("month")]
        public string Month { get; set; } = null!;

        [JsonPropertyName("income")]
        public decimal Income { get; set; }

        [JsonPropertyName("expense")]
        public decimal Expense { get; set; }

        [JsonPropertyName("net")]
        public decimal Net { get; set; }
    }
}
