using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Infrastructure.Services.Common
{
    /// <summary>
    /// Provides user context for multi-tenant operations.
    /// Centralizes user validation and society ID extraction.
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Gets the authenticated user and their society ID.
        /// Throws NotFoundException if user doesn't exist.
        /// </summary>
        /// <param name="userId">The user ID to validate</param>
        /// <returns>Tuple containing user entity and society ID</returns>
        Task<(User User, long SocietyId)> GetUserContextAsync(long userId);

        /// <summary>
        /// Gets the society ID for the authenticated user.
        /// Th rows NotFoundException if user doesn't exist.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The society ID</returns>
        Task<long> GetSocietyIdAsync(long userId);
    }
}
