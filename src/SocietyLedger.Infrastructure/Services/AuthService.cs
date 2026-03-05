using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.Auth;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;
using SocietyLedger.Infrastructure.Persistence.Repositories;
using SocietyLedger.Shared;

namespace SocietyLedger.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly ISocietyRepository _societyRepo;
        private readonly ITokenService _tokenService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly PasswordHasher _hasher;
        private readonly ILogger<AuthService> _logger;
        private readonly AppDbContext _db;

        public AuthService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            ISocietyRepository societyRepo,
            ITokenService tokenService,
            ISubscriptionService subscriptionService,
            PasswordHasher hasher,
            ILogger<AuthService> logger,
            AppDbContext db)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _societyRepo = societyRepo;
            _tokenService = tokenService;
            _subscriptionService = subscriptionService;
            _hasher = hasher;
            _logger = logger;
            _db = db;
        }

        // ===============================
        // LOGIN
        // ===============================
        public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await _userRepo.GetByUsernameOrEmailAsync(request.UsernameOrEmail);
            if (user == null)
                throw new AuthenticationException("Invalid credentials.");

            if (!_hasher.Verify(user.PasswordHash, request.Password))
                throw new AuthenticationException("Invalid credentials.");

            if (!user.IsActive)
                throw new AuthenticationException("User account is inactive.");

            var roles = new[] { user.Role?.DisplayName ?? "user" };
            var accessToken = _tokenService.GenerateAccessToken(user.Id, roles, out var accessExpires);
            var refreshPair = _tokenService.GenerateRefreshToken();

            var hashed = _hasher.Hash(refreshPair.Token);

            var refreshEntity = new refresh_token
            {
                user_id = user.Id,
                token_hash = hashed,
                jwt_id = Guid.NewGuid().ToString(),
                expires_at = refreshPair.ExpiresAt,
                created_at = DateTime.UtcNow,
                created_by_ip = ipAddress,
                is_revoked = false
            };

            // Use transaction to ensure atomicity of user update and token creation
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                user.LastLogin = DateTime.UtcNow;
                await _userRepo.UpdateAsync(user);

                _db.refresh_tokens.Add(refreshEntity);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            _logger.LogInformation("User {UserId} logged in successfully from IP {IP}", user.Id, ipAddress);

            return new LoginResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpires,
                RefreshToken = refreshPair.Token,
                RefreshTokenExpiresAt = refreshPair.ExpiresAt,
                Roles = roles,
                UserPublicId = user.PublicId,
                UserName = user.Name,
                Role = user.Role?.DisplayName,
                SocietyPublicId = user.SocietyPublicId,
                SocietyName = user.SocietyName,
                ForcePasswordChange = user.ForcePasswordChange
            };
        }


        // ===============================
        // REGISTER
        // ===============================
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string ipAddress)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await using var tx = await _db.Database.BeginTransactionAsync();

            var existingUser = await _userRepo.GetByUsernameOrEmailAsync(request.Email);
            if (existingUser != null)
                throw new DuplicateException("User", "email");

            var existingName = await _userRepo.GetByUsernameAsync(request.Name);
            if (existingName != null)
                throw new DuplicateException("User", "name");

            var society = Society.Create(
                request.SocietyName ?? "Default Society",
                request.SocietyAddress
            );

            await _societyRepo.AddAsync(society);

            var role = await _roleRepo.GetByCodeAsync(RoleCodes.SocietyAdmin)
                ?? throw new NotFoundException("Role", RoleCodes.SocietyAdmin);

            var user = new User
            {
                SocietyId = society.Id,
                Name = request.Name,
                Email = request.Email,
                PasswordHash = _hasher.Hash(request.Password),
                RoleId = role.Id,
                IsActive = true
                // PublicId, CreatedAt, UpdatedAt generated by database defaults
            };

            await _userRepo.AddAsync(user);

            var roles = new[] { role.Code ?? RoleCodes.SocietyAdmin };
            var accessToken = _tokenService.GenerateAccessToken(user.Id, roles, out var accessExpires);
            var refreshPair = _tokenService.GenerateRefreshToken();

            var refreshEntity = new refresh_token
            {
                user_id = user.Id,
                token_hash = _hasher.Hash(refreshPair.Token),
                jwt_id = Guid.NewGuid().ToString(),
                expires_at = refreshPair.ExpiresAt,
                created_at = DateTime.UtcNow,
                created_by_ip = ipAddress,
                is_revoked = false
            };

            _db.refresh_tokens.Add(refreshEntity);
            await _db.SaveChangesAsync();

            // Create trial subscription
            try
            {
                await _subscriptionService.CreateTrialSubscriptionAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create trial subscription for user {UserId}", user.Id);
                // Don't fail registration if trial creation fails
            }

            await tx.CommitAsync();

            _logger.LogInformation(
                "New user {UserId} registered new society {SocietyId} from {IP}",
                user.Id, society.Id, ipAddress);

            return new RegisterResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpires,
                RefreshToken = refreshPair.Token,
                RefreshTokenExpiresAt = refreshPair.ExpiresAt,
                Roles = roles,
                UserPublicId = user.PublicId,
                SocietyPublicId = society.PublicId,
                SocietyId = society.Id,
                UserId = user.Id
            };
        }


        // ===============================
        // REFRESH TOKEN
        // ===============================
        public async Task<LoginResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            var hashed = _hasher.Hash(token);

            var rt = await _db.refresh_tokens
                .Include(r => r.user)
                .ThenInclude(u => u.role)
                .Include(r => r.user)
                .ThenInclude(u => u.society)
                .FirstOrDefaultAsync(r => r.token_hash == hashed);

            if (rt == null || rt.is_revoked || rt.expires_at <= DateTime.UtcNow)
                throw new AuthenticationException("Invalid or expired refresh token");

            var newPair = _tokenService.GenerateRefreshToken();
            var newHashed = _hasher.Hash(newPair.Token);

            var newRt = new refresh_token
            {
                user_id = rt.user_id,
                token_hash = newHashed,
                jwt_id = Guid.NewGuid().ToString(),
                expires_at = newPair.ExpiresAt,
                created_at = DateTime.UtcNow,
                created_by_ip = ipAddress,
                is_revoked = false,
                replaced_by_token_hash = rt.token_hash
            };

            // Use transaction to ensure atomicity of token revocation and creation
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                rt.is_revoked = true;
                rt.revoked_at = DateTime.UtcNow;

                _db.refresh_tokens.Add(newRt);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var roles = new[] { rt.user?.role?.display_name ?? "User" };
            var accessToken = _tokenService.GenerateAccessToken(rt.user_id, roles, out var accessExpires);

            _logger.LogInformation("Refresh token rotated for user {UserId} from {Ip}", rt.user_id, ipAddress);

            return new LoginResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpires,
                RefreshToken = newPair.Token,
                RefreshTokenExpiresAt = newPair.ExpiresAt,
                Roles = roles,
                UserPublicId = rt.user?.public_id ?? Guid.Empty,
                UserName = rt.user?.name ?? string.Empty,
                Role = rt.user?.role?.display_name,
                SocietyPublicId = rt.user?.society?.public_id ?? Guid.Empty,
                SocietyName = rt.user?.society?.name ?? string.Empty,
                ForcePasswordChange = rt.user?.force_password_change ?? false
            };
        }

        // ===============================
        // REVOKE TOKEN
        // ===============================
        public async Task RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            var hashed = _hasher.Hash(token);
            var rt = await _db.refresh_tokens.FirstOrDefaultAsync(r => r.token_hash == hashed);
            if (rt == null)
                return;

            rt.is_revoked = true;
            rt.revoked_at = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked for user {UserId} from {Ip}", rt.user_id, ipAddress);
        }

        // ===============================
        // CHANGE PASSWORD
        // ===============================
        public async Task<ChangePasswordResponse> ChangePasswordAsync(long userId, ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId.ToString());

            if (!user.IsActive)
                throw new ConflictException("User account is inactive.");

            // Verify current password
            if (!_hasher.Verify(user.PasswordHash, request.CurrentPassword))
                throw new ValidationException(
                    ErrorMessages.VALIDATION_FAILED,
                    new Dictionary<string, string[]>
                    {
                        ["currentPassword"] = ["Current password is incorrect."]
                    });

            // Hash and update new password
            var newPasswordHash = _hasher.Hash(request.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.ForcePasswordChange = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user {UserId}", userId);

            return new ChangePasswordResponse
            {
                Message = "Password changed successfully.",
                ForcePasswordChange = false
            };
        }
    }
}
