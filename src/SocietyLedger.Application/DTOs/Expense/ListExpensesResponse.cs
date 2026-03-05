namespace SocietyLedger.Application.DTOs.Expense
{
    public class ListExpensesResponse
    {
        public List<ExpenseResponse> Expenses { get; set; } = new();
    }
}
