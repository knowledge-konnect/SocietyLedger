using System.Text.Json.Serialization;

namespace SocietyLedger.Application.DTOs.Reports
{
    public class ExpenseByCategoryDto
    {
        [JsonPropertyName("total_expense")]
        public decimal TotalExpense { get; set; }

        [JsonPropertyName("categories")]
        public List<ExpenseCategoryBreakdownDto> Categories { get; set; } = new();
    }

    public class ExpenseCategoryBreakdownDto
    {
        [JsonPropertyName("category")]
        public string Category { get; set; } = null!;

        [JsonPropertyName("category_code")]
        public string CategoryCode { get; set; } = null!;

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("total_entries")]
        public int TotalEntries { get; set; }

        [JsonPropertyName("first_expense_date")]
        public string? FirstExpenseDate { get; set; }

        [JsonPropertyName("last_expense_date")]
        public string? LastExpenseDate { get; set; }
    }
}
