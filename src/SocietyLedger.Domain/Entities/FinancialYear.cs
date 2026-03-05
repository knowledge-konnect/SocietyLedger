namespace SocietyLedger.Domain.Entities
{
    public class FinancialYear
    {
        public Guid Id { get; set; }
        public Guid SocietyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Society? Society { get; set; }
    }
}
