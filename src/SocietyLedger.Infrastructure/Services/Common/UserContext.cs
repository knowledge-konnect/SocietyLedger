using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Domain.Exceptions;

namespace SocietyLedger.Infrastructure.Services.Common
{
    /// <summary>
    /// Implementation of user context for multi-tenant operations.
    /// Centralizes user validation and caching to reduce database calls.
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IUserRepository _userRepository;

        public UserContext(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<(User User, long SocietyId)> GetUserContextAsync(long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId.ToString());
            }

            return (user, user.SocietyId);
        }

        public async Task<long> GetSocietyIdAsync(long userId)
        {
            var (_, societyId) = await GetUserContextAsync(userId);
            return societyId;
        }
    }
}
