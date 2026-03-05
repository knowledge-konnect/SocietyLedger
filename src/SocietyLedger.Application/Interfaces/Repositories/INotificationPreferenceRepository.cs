using SocietyLedger.Application.DTOs.Notification;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface INotificationPreferenceRepository
    {
        /// <summary>Gets notification preferences for a user, or null if none exist.</summary>
        Task<NotificationPreferenceResponse?> GetByUserIdAsync(long userId);

        /// <summary>Creates or updates notification preferences for a user (upsert).</summary>
        Task<NotificationPreferenceResponse> UpsertAsync(long userId, UpdateNotificationPreferencesRequest request);
    }
}
