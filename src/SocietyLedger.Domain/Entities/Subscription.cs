namespace SocietyLedger.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid PlanId { get; set; }
        public string Status { get; set; } = null!;
        public decimal SubscribedAmount { get; set; }
        public string? Currency { get; set; }
        public DateTime? CurrentPeriodStart { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
        public DateTime? TrialStart { get; set; }
        public DateTime? TrialEnd { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Plan? Plan { get; set; }
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }

    public class Plan
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal MonthlyAmount { get; set; }
        public string Currency { get; set; } = null!;
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class Invoice
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid? SubscriptionId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string InvoiceType { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public string Status { get; set; } = null!;
        public DateOnly? PeriodStart { get; set; }
        public DateOnly? PeriodEnd { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentReference { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Subscription? Subscription { get; set; }
    }

    public class SubscriptionEvent
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid? SubscriptionId { get; set; }
        public string EventType { get; set; } = null!;
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public decimal? Amount { get; set; }
        public string? Metadata { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        public Subscription? Subscription { get; set; }
    }
}