namespace SocietyLedger.Application.DTOs.Flat
{
    public class FlatLedgerResponse
    {
        public Guid FlatPublicId { get; set; }
        public string FlatNo { get; set; } = null!;
        public string? OwnerName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<FlatLedgerEntryDto> Entries { get; set; } = new List<FlatLedgerEntryDto>();
    }
}
