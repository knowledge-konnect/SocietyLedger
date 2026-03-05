using SocietyLedger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface ISocietyRepository
    {
        /// <summary>
        /// Adds a new Society to persistence (but does not necessarily save changes).
        /// </summary>
        Task AddAsync(Society society);

        /// <summary>
        /// Retrieves a Society by its public Id (UUID).
        /// Returns null if not found.
        /// </summary>
        Task<Society?> GetByPublicIdAsync(Guid publicId);

        /// <summary>
        /// Retrieves a Society by its Id.
        /// Returns null if not found.
        /// </summary>
        Task<Society?> GetByIdAsync(long id);

        /// <summary>
        /// Persists any pending changes in the underlying context.
        /// </summary>
        Task SaveChangesAsync();
    }
}

