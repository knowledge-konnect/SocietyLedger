namespace SocietyLedger.Domain.Entities
{
    public class OpeningBalanceBatch
    {
        public Guid Id { get; set; }
        public Guid SocietyId { get; set; }
        public Guid FinancialYearId { get; set; }
        public decimal SocietyOpeningAmount { get; set; }
        public DateTime AppliedAt { get; set; }
        public Guid AppliedBy { get; set; }
        public bool IsLocked { get; set; }

        // Navigation property
        public FinancialYear? FinancialYear { get; set; }
    }
}
