namespace SocietyLedger.Application.DTOs.Expense
{
    /// <summary>
    /// Internal DTO representing expense entity data from repository.
    /// Used to avoid circular dependency between Application and Infrastructure layers.
    /// </summary>
    public class ExpenseEntity
    {
        public Guid PublicId { get; set; }
        public long SocietyId { get; set; }
        public Guid SocietyPublicId { get; set; }
        public DateOnly DateIncurred { get; set; }
        public string CategoryCode { get; set; } = null!;
        public string? Vendor { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public long? AttachmentId { get; set; }
        public long? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public long? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
