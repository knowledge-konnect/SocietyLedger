namespace SocietyLedger.Application.DTOs.OpeningBalance
{
    public class OpeningBalanceRequest
    {
        /// <summary>
        /// The financial date of the opening entry (e.g. the actual start-of-books date).
        /// Must be >= society.onboarding_date and <= today.
        /// Stored as transaction_date — never derived from created_at.
        /// </summary>
        public DateOnly TransactionDate { get; set; }

        public decimal society_opening_amount { get; set; }
        public List<OpeningBalanceItemDto> items { get; set; } = new List<OpeningBalanceItemDto>();
        public List<OpeningBalanceItemDto> flat_items { get; set; } = new List<OpeningBalanceItemDto>();
    }
}
