using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.DTOs.Expense;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;
using SocietyLedger.Infrastructure.Persistence.Extensions;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _db;

        public ExpenseRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<ExpenseEntity?> GetByPublicIdAsync(Guid publicId, long societyId)
        {
            var expense = await _db.expenses
                .ForSociety(societyId)
                .Include(e => e.created_byNavigation)
                .Include(e => e.approved_byNavigation)
                .Include(e => e.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.public_id == publicId);
            
            return expense != null ? MapToDto(expense) : null;
        }

        public async Task<IEnumerable<ExpenseEntity>> GetBySocietyIdAsync(long societyId)
        {
            var expenses = await _db.expenses
                .ForSociety(societyId)
                .Include(e => e.created_byNavigation)
                .Include(e => e.approved_byNavigation)
                .Include(e => e.society)
                .AsNoTracking()
                .OrderByDescending(e => e.date_incurred)
                .ToListAsync();
            
            return expenses.Select(MapToDto);
        }

        public async Task<IEnumerable<ExpenseEntity>> GetByDateRangeAsync(long societyId, DateOnly startDate, DateOnly endDate)
        {
            var expenses = await _db.expenses
                .ForSociety(societyId)
                .Where(e => e.date_incurred >= startDate && e.date_incurred <= endDate)
                .Include(e => e.created_byNavigation)
                .Include(e => e.approved_byNavigation)
                .Include(e => e.society)
                .AsNoTracking()
                .OrderByDescending(e => e.date_incurred)
                .ToListAsync();
            
            return expenses.Select(MapToDto);
        }

        public async Task<IEnumerable<ExpenseEntity>> GetByCategoryAsync(long societyId, string categoryCode)
        {
            var expenses = await _db.expenses
                .ForSociety(societyId)
                .Where(e => e.category_code == categoryCode)
                .Include(e => e.created_byNavigation)
                .Include(e => e.approved_byNavigation)
                .Include(e => e.society)
                .AsNoTracking()
                .OrderByDescending(e => e.date_incurred)
                .ToListAsync();
            
            return expenses.Select(MapToDto);
        }

        public async Task AddAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _db.expenses.AddAsync((expense)entity);
        }

        public async Task UpdateAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _db.expenses.Update((expense)entity);
        }

        public async Task DeleteByPublicIdAsync(Guid publicId, long societyId)
        {
            var entity = await _db.expenses
                .ForSociety(societyId)
                .FirstOrDefaultAsync(e => e.public_id == publicId);
            
            if (entity != null)
            {
                entity.is_deleted = true;
                entity.deleted_at = DateTime.UtcNow;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        private ExpenseEntity MapToDto(expense entity)
        {
            return new ExpenseEntity
            {
                PublicId = entity.public_id,
                SocietyId = entity.society_id,
                SocietyPublicId = entity.society?.public_id ?? Guid.Empty,
                DateIncurred = entity.date_incurred,
                CategoryCode = entity.category_code,
                Vendor = entity.vendor,
                Description = entity.description,
                Amount = entity.amount,
                AttachmentId = entity.attachment_id,
                ApprovedBy = entity.approved_by,
                ApprovedByName = entity.approved_byNavigation?.name,
                CreatedBy = entity.created_by,
                CreatedByName = entity.created_byNavigation?.name,
                CreatedAt = entity.created_at
            };
        }
    }
}
