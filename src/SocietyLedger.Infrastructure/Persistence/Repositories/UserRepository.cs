using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Extensions;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Get a user by username within a specific society (society isolation).
        /// </summary>
        public async Task<User?> GetByUsernameAndSocietyAsync(string username, long societyId)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var lowered = username.ToLowerInvariant();

            var efUser = await _db.users
                .ForSociety(societyId)
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.username ?? string.Empty).ToLower() == lowered);

            return efUser?.ToDomain();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            var efUser = await _db.users
                .ExcludeDeleted()
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.id == id);

            return efUser?.ToDomain();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var lowered = username.ToLowerInvariant();

            var efUser = await _db.users
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.name ?? string.Empty).ToLower() == lowered && !u.is_deleted);

            return efUser?.ToDomain();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var lowered = email.ToLowerInvariant();

            var efUser = await _db.users
                .ExcludeDeleted()
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.email ?? string.Empty).ToLower() == lowered);

            return efUser?.ToDomain();
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            var lowered = usernameOrEmail.ToLowerInvariant();
            var row = await (from u in _db.users.AsNoTracking()
                             join r in _db.roles.AsNoTracking() on u.role_id equals r.id into rg
                             from r in rg.DefaultIfEmpty()
                             join s in _db.societies.AsNoTracking() on u.society_id equals s.id into sg
                             from s in sg.DefaultIfEmpty()
                             where ((u.email ?? string.Empty).ToLower() == lowered) ||
                                   ((u.username ?? string.Empty).ToLower() == lowered)
                                   && !u.is_deleted
                             select new
                             {
                                 UserEntity = u,
                                 RoleEntity = r,
                                 SocietyName = s != null ? s.name : null,
                                 SocietyPublicId = s != null ? s.public_id : Guid.Empty
                             })
                             .FirstOrDefaultAsync();

            if (row == null) return null;

            var domainUser = row.UserEntity.ToDomain();
            domainUser.Role = row.RoleEntity != null ? row.RoleEntity.ToDomain() : null;
            domainUser.SocietyName = row.SocietyName ?? string.Empty;
            domainUser.SocietyPublicId = row.SocietyPublicId;
            return domainUser;

        }

        /// <summary>
        /// Adds a new user to the database and saves immediately.
        /// Copies the generated DB Id and PublicId back to the domain model.
        /// </summary>
        public async Task AddAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Map domain -> EF
            var entity = user.ToEntity();

            await _db.users.AddAsync(entity);

            // Persist to generate the ID
            await _db.SaveChangesAsync();

            // Copy generated fields back to domain
            user.Id = entity.id;
            user.PublicId = entity.public_id;
        }

        /// <summary>
        /// Updates an existing user (does not call SaveChanges automatically).
        /// </summary>
        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var efUser = await _db.users.FirstOrDefaultAsync(u => u.id == user.Id && !u.is_deleted);
            if (efUser == null)
                throw new InvalidOperationException($"User with id {user.Id} not found.");

            efUser.name = user.Name;
            efUser.email = user.Email;
            efUser.mobile = user.Mobile;
            efUser.role_id = user.RoleId;
            efUser.password_hash = user.PasswordHash;
            efUser.is_active = user.IsActive;
            efUser.force_password_change = user.ForcePasswordChange;
            efUser.last_login = user.LastLogin;
            efUser.updated_at = user.UpdatedAt;
        }

        /// <summary>
        /// Get a user by public ID within a specific society for tenant isolation.
        /// </summary>
        public async Task<User?> GetByPublicIdAsync(Guid publicId, long societyId)
        {
            var efUser = await _db.users
                .ForSociety(societyId)
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.public_id == publicId);

            return efUser?.ToDomain();
        }

        /// <summary>
        /// Get all users for a specific society.
        /// </summary>
        public async Task<IEnumerable<User>> GetBySocietyIdAsync(long societyId)
        {
            var users = await _db.users
                .ForSociety(societyId)
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .OrderBy(u => u.name)
                .ToListAsync();

            return users.Select(u => u.ToDomain());
        }

        /// <summary>
        /// Get a user by email within a specific society (society isolation).
        /// </summary>
        public async Task<User?> GetByEmailAndSocietyAsync(string email, long societyId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var lowered = email.ToLowerInvariant();

            var efUser = await _db.users
                .ForSociety(societyId)
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.email ?? string.Empty).ToLower() == lowered);

            return efUser?.ToDomain();
        }

        /// <summary>
        /// Get a user by mobile within a specific society (society isolation).
        /// </summary>
        public async Task<User?> GetByMobileAndSocietyAsync(string mobile, long societyId)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return null;

            var efUser = await _db.users
                .ForSociety(societyId)
                .Include(u => u.role)
                .Include(u => u.society)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.mobile == mobile);

            return efUser?.ToDomain();
        }

        /// <summary>
        /// Soft delete a user by public ID (sets is_deleted = true) with society_id verification for tenant isolation.
        /// </summary>
        public async Task<bool> SoftDeleteByPublicIdAsync(Guid publicId, long societyId)
        {
            var efUser = await _db.users
                .ForSociety(societyId)
                .FirstOrDefaultAsync(u => u.public_id == publicId);
            
            if (efUser == null)
                return false;

            efUser.is_deleted = true;
            efUser.deleted_at = DateTime.UtcNow;
            efUser.updated_at = DateTime.UtcNow;
            return true;
        }

        /// <summary>
        /// Commits pending changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
