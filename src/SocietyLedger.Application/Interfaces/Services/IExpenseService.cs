using SocietyLedger.Application.DTOs.Expense;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<ExpenseResponse> CreateExpenseAsync(long userId, CreateExpenseRequest request);
        Task<ExpenseResponse> GetExpenseAsync(Guid publicId, long userId);
        Task<IEnumerable<ExpenseResponse>> GetExpensesBySocietyAsync(long userId);
        Task<IEnumerable<ExpenseResponse>> GetExpensesByDateRangeAsync(long userId, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<ExpenseResponse>> GetExpensesByCategoryAsync(long userId, string categoryCode);
        Task<IEnumerable<ExpenseCategoryResponse>> GetExpenseCategoriesAsync();
        Task<ExpenseResponse> UpdateExpenseAsync(Guid publicId, long userId, UpdateExpenseRequest request);
        Task DeleteExpenseAsync(Guid publicId, long userId);
    }
}
