using Serilog;
using SocietyLedger.Application.DTOs.Expense;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepo;
        private readonly IUserContext _userContext;
        private readonly AppDbContext _db;

        public ExpenseService(
            IExpenseRepository expenseRepo,
            IUserContext userContext,
            AppDbContext db)
        {
            _expenseRepo = expenseRepo ?? throw new ArgumentNullException(nameof(expenseRepo));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<ExpenseResponse> CreateExpenseAsync(long userId, CreateExpenseRequest request)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            // Validate business date against the society's financial epoch.
            // This must happen in the service layer because it requires a DB round-trip.
            var onboardingDate = await GetOnboardingDateAsync(societyId);
            if (request.Date < onboardingDate)
                throw new ValidationException(
                    $"Expense date ({request.Date:yyyy-MM-dd}) cannot be earlier than " +
                    $"the society onboarding date ({onboardingDate:yyyy-MM-dd}).");

            // Create expense with public_id and created_by
            var expenseEntity = new expense
            {
                public_id = Guid.NewGuid(),
                society_id = societyId,
                date_incurred = request.Date,
                category_code = request.CategoryCode,
                vendor = request.Vendor,
                description = request.Description,
                amount = request.Amount,
                created_by = userId,
                created_at = DateTime.UtcNow
            };

            await _expenseRepo.AddAsync(expenseEntity);
            await _expenseRepo.SaveChangesAsync();

            Log.Information("Expense created successfully by user {UserId} for society {SocietyId}", userId, societyId);
            
            // Reload to get navigation properties
            var createdExpense = await _expenseRepo.GetByPublicIdAsync(expenseEntity.public_id, societyId);
            if (createdExpense == null)
                throw new InvalidOperationException("Failed to retrieve created expense");
            
            return MapToResponse(createdExpense);
        }

        public async Task<ExpenseResponse> GetExpenseAsync(Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            
            var expenseEntity = await _expenseRepo.GetByPublicIdAsync(publicId, societyId);
            if (expenseEntity == null)
                throw new NotFoundException("Expense", publicId.ToString());

            return MapToResponse(expenseEntity);
        }

        public async Task<IEnumerable<ExpenseResponse>> GetExpensesBySocietyAsync(long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            
            var expenses = await _expenseRepo.GetBySocietyIdAsync(societyId);
            return expenses.Select(MapToResponse);
        }

        public async Task<IEnumerable<ExpenseResponse>> GetExpensesByDateRangeAsync(long userId, DateOnly startDate, DateOnly endDate)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            
            var expenses = await _expenseRepo.GetByDateRangeAsync(societyId, startDate, endDate);
            return expenses.Select(MapToResponse);
        }

        public async Task<IEnumerable<ExpenseResponse>> GetExpensesByCategoryAsync(long userId, string categoryCode)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);
            
            var expenses = await _expenseRepo.GetByCategoryAsync(societyId, categoryCode);
            return expenses.Select(MapToResponse);
        }

        public async Task<IEnumerable<ExpenseCategoryResponse>> GetExpenseCategoriesAsync()
        {
            var categories = await _db.expense_categories
                .AsNoTracking()
                .OrderBy(c => c.display_name)
                .ToListAsync();

            return categories.Select(c => new ExpenseCategoryResponse
            {
                Code = c.code,
                DisplayName = c.display_name
            });
        }

        public async Task<ExpenseResponse> UpdateExpenseAsync(Guid publicId, long userId, UpdateExpenseRequest request)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var expenseEntity = await _expenseRepo.GetByPublicIdAsync(publicId, societyId);
            if (expenseEntity == null)
                throw new NotFoundException("Expense", publicId.ToString());

            // Get tracked entity for update
            var trackedExpense = await _db.expenses.FirstOrDefaultAsync(e => e.public_id == publicId && e.society_id == societyId);
            if (trackedExpense == null)
                throw new NotFoundException("Expense", publicId.ToString());

            // Validate updated date against the society's financial epoch.
            if (request.Date.HasValue)
            {
                var onboardingDate = await GetOnboardingDateAsync(societyId);
                if (request.Date.Value < onboardingDate)
                    throw new ValidationException(
                        $"Expense date ({request.Date.Value:yyyy-MM-dd}) cannot be earlier than " +
                        $"the society onboarding date ({onboardingDate:yyyy-MM-dd}).");
            }

            // Update fields if provided
            if (request.Date.HasValue)
                trackedExpense.date_incurred = request.Date.Value;

            if (request.CategoryCode != null)
                trackedExpense.category_code = request.CategoryCode;

            if (request.Vendor != null)
                trackedExpense.vendor = request.Vendor;

            if (request.Description != null)
                trackedExpense.description = request.Description;

            if (request.Amount.HasValue)
                trackedExpense.amount = request.Amount.Value;

            await _expenseRepo.UpdateAsync(trackedExpense);
            await _expenseRepo.SaveChangesAsync();

            // Reload with navigation properties
            var updatedExpense = await _expenseRepo.GetByPublicIdAsync(publicId, societyId);
            if (updatedExpense == null)
                throw new InvalidOperationException("Failed to retrieve updated expense");
            
            return MapToResponse(updatedExpense);
        }

        public async Task DeleteExpenseAsync(Guid publicId, long userId)
        {
            var societyId = await _userContext.GetSocietyIdAsync(userId);

            var expenseEntity = await _expenseRepo.GetByPublicIdAsync(publicId, societyId);
            if (expenseEntity == null)
                throw new NotFoundException("Expense", publicId.ToString());

            await _expenseRepo.DeleteByPublicIdAsync(publicId, societyId);
            await _expenseRepo.SaveChangesAsync();

            Log.Information("Expense deleted successfully: {PublicId}", publicId);
        }

        private ExpenseResponse MapToResponse(ExpenseEntity entity)
        {
            return new ExpenseResponse
            {
                PublicId = entity.PublicId,
                SocietyPublicId = entity.SocietyPublicId,
                DateIncurred = entity.DateIncurred,
                CategoryCode = entity.CategoryCode,
                Vendor = entity.Vendor,
                Description = entity.Description,
                Amount = entity.Amount,
                ApprovedByName = entity.ApprovedByName,
                CreatedByName = entity.CreatedByName,
                CreatedAt = entity.CreatedAt
            };
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the society's onboarding date. Throws <see cref="NotFoundException"/>
        /// if the society is not found (soft-delete check included).
        /// Result is used to reject expense dates earlier than the financial epoch.
        /// </summary>
        private async Task<DateOnly> GetOnboardingDateAsync(long societyId)
        {
            var date = await _db.societies
                .AsNoTracking()
                .Where(s => s.id == societyId && !s.is_deleted)
                .Select(s => (DateOnly?)s.onboarding_date)
                .FirstOrDefaultAsync();

            if (date is null)
                throw new NotFoundException("Society", societyId.ToString());

            return date.Value;
        }    }
}