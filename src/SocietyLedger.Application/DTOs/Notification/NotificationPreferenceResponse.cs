namespace SocietyLedger.Application.DTOs.Notification
{
    /// <summary>
    /// Notification preference settings for a user.
    /// </summary>
    public class NotificationPreferenceResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public bool PaymentReminders { get; set; }
        public bool BillGenerated { get; set; }
        public bool ExpenseUpdates { get; set; }
        public bool MonthlyReports { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
