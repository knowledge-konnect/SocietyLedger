namespace SocietyLedger.Domain.Entities
{
    public class Flat
    {
        public long Id { get; set; }
        public Guid PublicId { get; set; }
        public long SocietyId { get; set; }
        public Guid SocietyPublicId { get; set; }
        public string FlatNo { get; set; } = null!;
        public string? OwnerName { get; set; }
        public string? ContactMobile { get; set; }
        public string? ContactEmail { get; set; }
        public decimal MaintenanceAmount { get; set; }
        public short? StatusId { get; set; }              
        public string StatusName { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
