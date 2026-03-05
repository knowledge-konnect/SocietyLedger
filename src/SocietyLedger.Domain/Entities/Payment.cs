namespace SocietyLedger.Domain.Entities
{
    public class Payment
    {
        public long Id { get; set; }
        public Guid PublicId { get; set; }
        public long SocietyId { get; set; }
        public long? BillId { get; set; }
        public long? FlatId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DatePaid { get; set; }
        public string? ModeCode { get; set; }
        public string? Reference { get; set; }
        public string? ReceiptUrl { get; set; }
        public long? RecordedBy { get; set; }
        public string? IdempotencyKey { get; set; }
        public long? ReversedByPaymentId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Razorpay fields
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }
        public string? PaymentType { get; set; } // 'bill' or 'subscription'
        public DateTime? VerifiedAt { get; set; }

        // Navigation
        public Society? Society { get; set; }
        public Flat? Flat { get; set; }
        public User? RecordedByNavigation { get; set; }
        public Payment? ReversedByPayment { get; set; }
    }
}