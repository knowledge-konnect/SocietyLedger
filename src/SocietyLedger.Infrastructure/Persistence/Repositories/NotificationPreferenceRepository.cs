using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.DTOs.Notification;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class NotificationPreferenceRepository : INotificationPreferenceRepository
    {
        private readonly AppDbContext _db;

        public NotificationPreferenceRepository(AppDbContext db) => _db = db;

        public async Task<NotificationPreferenceResponse?> GetByUserIdAsync(long userId)
        {
            var pref = await _db.notification_preferences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.user_id == userId);

            return pref == null ? null : MapToResponse(pref);
        }

        public async Task<NotificationPreferenceResponse> UpsertAsync(long userId, UpdateNotificationPreferencesRequest request)
        {
            var pref = await _db.notification_preferences
                .FirstOrDefaultAsync(p => p.user_id == userId);

            if (pref == null)
            {
                pref = new notification_preference
                {
                    public_id = Guid.NewGuid(),
                    user_id = userId,
                    // Apply provided values, default to true for all notification types
                    payment_reminders = request.PaymentReminders ?? true,
                    bill_generated = request.BillGenerated ?? true,
                    expense_updates = request.ExpenseUpdates ?? true,
                    monthly_reports = request.MonthlyReports ?? true,
                    updated_at = DateTime.UtcNow
                };
                _db.notification_preferences.Add(pref);
            }
            else
            {
                // Partial update: only update provided fields
                if (request.PaymentReminders.HasValue)
                    pref.payment_reminders = request.PaymentReminders.Value;
                if (request.BillGenerated.HasValue)
                    pref.bill_generated = request.BillGenerated.Value;
                if (request.ExpenseUpdates.HasValue)
                    pref.expense_updates = request.ExpenseUpdates.Value;
                if (request.MonthlyReports.HasValue)
                    pref.monthly_reports = request.MonthlyReports.Value;

                pref.updated_at = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return MapToResponse(pref);
        }

        private static NotificationPreferenceResponse MapToResponse(notification_preference pref) =>
            new NotificationPreferenceResponse
            {
                Id = pref.id,
                UserId = pref.user_id,
                PaymentReminders = pref.payment_reminders,
                BillGenerated = pref.bill_generated,
                ExpenseUpdates = pref.expense_updates,
                MonthlyReports = pref.monthly_reports,
                UpdatedAt = pref.updated_at
            };
    }
}
