using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.Notification;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;

namespace SocietyLedger.Infrastructure.Services
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly INotificationPreferenceRepository _prefRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<NotificationPreferenceService> _logger;

        public NotificationPreferenceService(
            INotificationPreferenceRepository prefRepo,
            IUserRepository userRepo,
            ILogger<NotificationPreferenceService> logger)
        {
            _prefRepo = prefRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<NotificationPreferenceResponse> GetPreferencesAsync(long userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
                throw new NotFoundException("User", userId.ToString());

            var pref = await _prefRepo.GetByUserIdAsync(userId);

            // Return defaults if no preferences configured yet
            if (pref == null)
            {
                return new NotificationPreferenceResponse
                {
                    Id = 0,
                    UserId = userId,
                    PaymentReminders = true,
                    BillGenerated = true,
                    ExpenseUpdates = true,
                    MonthlyReports = true,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            return pref;
        }

        public async Task<NotificationPreferenceResponse> UpdatePreferencesAsync(long userId, UpdateNotificationPreferencesRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
                throw new NotFoundException("User", userId.ToString());

            var result = await _prefRepo.UpsertAsync(userId, request);
            _logger.LogInformation("Notification preferences updated for user {UserId}", userId);
            return result;
        }
    }
}
