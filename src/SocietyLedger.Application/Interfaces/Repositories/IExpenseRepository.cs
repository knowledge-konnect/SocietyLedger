namespace SocietyLedger.Application.Interfaces.Repositories
{
    using SocietyLedger.Application.DTOs.Expense;

    public interface IExpenseRepository
    {
        Task<ExpenseEntity?> GetByPublicIdAsync(Guid publicId, long societyId);
        Task<IEnumerable<ExpenseEntity>> GetBySocietyIdAsync(long societyId);
        Task<IEnumerable<ExpenseEntity>> GetByDateRangeAsync(long societyId, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<ExpenseEntity>> GetByCategoryAsync(long societyId, string categoryCode);
        Task AddAsync(object entity);
        Task UpdateAsync(object entity);
        Task DeleteByPublicIdAsync(Guid publicId, long societyId);
        Task SaveChangesAsync();
    }
}
