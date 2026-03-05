namespace SocietyLedger.Application.DTOs.Notification
{
    /// <summary>
    /// Request DTO for updating notification preferences. All fields are optional (partial update).
    /// </summary>
    public class UpdateNotificationPreferencesRequest
    {
        public bool? PaymentReminders { get; set; }
        public bool? BillGenerated { get; set; }
        public bool? ExpenseUpdates { get; set; }
        public bool? MonthlyReports { get; set; }
    }
}
