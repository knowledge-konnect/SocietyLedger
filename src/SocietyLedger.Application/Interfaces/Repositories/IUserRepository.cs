using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get a user by username within a specific society (society isolation).
        /// </summary>
        Task<User?> GetByUsernameAndSocietyAsync(string username, long societyId);
        /// <summary>
        /// Get a user by database id.
        /// </summary>
        Task<User?> GetByIdAsync(long id);

        /// <summary>
        /// Get a user by username (case-insensitive).
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Get a user by email (case-insensitive).
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Get a user by username or email (case-insensitive).
        /// </summary>
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);

        /// <summary>
        /// Get a user by public ID (UUID) within a specific society for tenant isolation.
        /// </summary>
        Task<User?> GetByPublicIdAsync(Guid publicId, long societyId);

        /// <summary>
        /// Get all users for a specific society.
        /// </summary>
        Task<IEnumerable<User>> GetBySocietyIdAsync(long societyId);

        /// <summary>
        /// Get a user by email within a specific society (society isolation).
        /// </summary>
        Task<User?> GetByEmailAndSocietyAsync(string email, long societyId);

        /// <summary>
        /// Get a user by mobile within a specific society (society isolation).
        /// </summary>
        Task<User?> GetByMobileAndSocietyAsync(string mobile, long societyId);

        /// <summary>
        /// Adds a new user to persistence (but does not necessarily save changes).
        /// </summary>
        Task AddAsync(User user);

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        Task UpdateAsync(User user);

        /// <summary>
        /// Soft delete a user by public ID (sets is_deleted = true) with society_id verification.
        /// </summary>
        Task<bool> SoftDeleteByPublicIdAsync(Guid publicId, long societyId);

        /// <summary>
        /// Persists pending changes in the underlying context.
        /// </summary>
        Task SaveChangesAsync();
    }
}
