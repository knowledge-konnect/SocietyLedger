namespace SocietyLedger.Application.DTOs.Flat
{
    public class FlatLedgerEntryDto
    {
        public DateTime Date { get; set; }
        public string EntryType { get; set; } = null!; // "maintenance" or "payment"
        public string? Period { get; set; } // For maintenance charges (yyyy-MM)
        public decimal Charge { get; set; } // Debit amount
        public decimal Payment { get; set; } // Credit amount
        public decimal Balance { get; set; } // Running balance
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
