namespace SocietyLedger.Application.DTOs.OpeningBalance
{
    public class OpeningBalanceStatusResponse
    {
        public bool IsApplied { get; set; }

        /// <summary>
        /// Financial event date of the opening entry (transaction_date).
        /// Use this date for any financial display or reporting.
        /// </summary>
        public DateOnly? TransactionDate { get; set; }

        /// <summary>
        /// System audit timestamp — when the row was physically created (created_at).
        /// For audit trails only; do not use for financial calculations.
        /// </summary>
        public DateTime? AuditCreatedAt { get; set; }

        public string? AppliedBy { get; set; }
    }
}
