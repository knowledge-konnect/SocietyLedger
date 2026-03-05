namespace SocietyLedger.Application.DTOs.Expense
{
    public class ExpenseCategoriesResponse
    {
        public List<ExpenseCategoryResponse> Categories { get; set; } = new();
    }
}
