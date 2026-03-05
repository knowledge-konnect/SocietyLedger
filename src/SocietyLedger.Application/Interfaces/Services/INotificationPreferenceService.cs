using SocietyLedger.Application.DTOs.Notification;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface INotificationPreferenceService
    {
        /// <summary>Get notification preferences for the authenticated user. Returns defaults if none configured yet.</summary>
        Task<NotificationPreferenceResponse> GetPreferencesAsync(long userId);

        /// <summary>Update notification preferences for the authenticated user (partial update).</summary>
        Task<NotificationPreferenceResponse> UpdatePreferencesAsync(long userId, UpdateNotificationPreferencesRequest request);
    }
}
