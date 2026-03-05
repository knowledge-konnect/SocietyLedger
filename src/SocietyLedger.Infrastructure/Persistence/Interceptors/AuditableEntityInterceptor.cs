using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Interceptors
{
    /// <summary>
    /// SaveChanges interceptor that automatically handles:
    /// 1. Generating Version 7 GUIDs for new entities
    /// 2. Setting CreatedAt for new entities
    /// 3. Updating UpdatedAt for modified entities
    /// </summary>
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    // Don't set PublicId - let database default (gen_random_uuid()) generate it
                    // Don't set CreatedAt - let database default (now()) generate it
                    
                    // Only set UpdatedAt
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // For modified entities, update UpdatedAt
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;

                    // Prevent modifying CreatedAt
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                }
            }
        }
    }
}
