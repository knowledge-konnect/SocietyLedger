using Microsoft.EntityFrameworkCore;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Extensions
{
    /// <summary>
    /// Extension methods for IQueryable to provide common multi-tenant and soft-delete filtering.
    /// Promotes DRY principle by centralizing repeated query patterns.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filters query by society ID and excludes soft-deleted records.
        /// </summary>
        public static IQueryable<T> ForSociety<T>(this IQueryable<T> query, long societyId) where T : class
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            
            // society_id == societyId
            var societyProperty = System.Linq.Expressions.Expression.Property(parameter, "society_id");
            var societyConstant = System.Linq.Expressions.Expression.Constant(societyId);
            var societyEquals = System.Linq.Expressions.Expression.Equal(societyProperty, societyConstant);
            
            // !is_deleted
            var isDeletedProperty = System.Linq.Expressions.Expression.Property(parameter, "is_deleted");
            var notDeleted = System.Linq.Expressions.Expression.Not(isDeletedProperty);
            
            // Combined: society_id == societyId && !is_deleted
            var combined = System.Linq.Expressions.Expression.AndAlso(societyEquals, notDeleted);
            
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(combined, parameter);
            
            return query.Where(lambda);
        }

        /// <summary>
        /// Filters query to exclude soft-deleted records.
        /// </summary>
        public static IQueryable<T> ExcludeDeleted<T>(this IQueryable<T> query) where T : class
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            var isDeletedProperty = System.Linq.Expressions.Expression.Property(parameter, "is_deleted");
            var notDeleted = System.Linq.Expressions.Expression.Not(isDeletedProperty);
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(notDeleted, parameter);
            
            return query.Where(lambda);
        }

        /// <summary>
        /// Filters users by society ID and excludes soft-deleted records.
        /// </summary>
        public static IQueryable<user> ForSociety(this IQueryable<user> query, long societyId)
        {
            return query.Where(u => u.society_id == societyId && !u.is_deleted);
        }

        /// <summary>
        /// Filters flats by society ID and excludes soft-deleted records.
        /// </summary>
        public static IQueryable<flat> ForSociety(this IQueryable<flat> query, long societyId)
        {
            return query.Where(f => f.society_id == societyId && !f.is_deleted);
        }

        /// <summary>
        /// Filters expenses by society ID and excludes soft-deleted records.
        /// </summary>
        public static IQueryable<expense> ForSociety(this IQueryable<expense> query, long societyId)
        {
            return query.Where(e => e.society_id == societyId && !e.is_deleted);
        }

        /// <summary>
        /// Filters maintenance payments by society ID and excludes soft-deleted records.
        /// </summary>
        public static IQueryable<maintenance_payment> ForSociety(this IQueryable<maintenance_payment> query, long societyId)
        {
            return query.Where(mp => mp.society_id == societyId && !mp.is_deleted);
        }

        /// <summary>
        /// Excludes soft-deleted user records.
        /// </summary>
        public static IQueryable<user> ExcludeDeleted(this IQueryable<user> query)
        {
            return query.Where(u => !u.is_deleted);
        }

        /// <summary>
        /// Excludes soft-deleted flat records.
        /// </summary>
        public static IQueryable<flat> ExcludeDeleted(this IQueryable<flat> query)
        {
            return query.Where(f => !f.is_deleted);
        }

        /// <summary>
        /// Excludes soft-deleted expense records.
        /// </summary>
        public static IQueryable<expense> ExcludeDeleted(this IQueryable<expense> query)
        {
            return query.Where(e => !e.is_deleted);
        }

        /// <summary>
        /// Excludes soft-deleted maintenance payment records.
        /// </summary>
        public static IQueryable<maintenance_payment> ExcludeDeleted(this IQueryable<maintenance_payment> query)
        {
            return query.Where(mp => !mp.is_deleted);
        }
    }
}
