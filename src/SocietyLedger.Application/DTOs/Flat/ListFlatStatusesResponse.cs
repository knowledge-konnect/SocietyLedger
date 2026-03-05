namespace SocietyLedger.Application.DTOs.Flat
{
    /// <summary>
    /// Response wrapper for list of flat statuses.
    /// </summary>
    public class ListFlatStatusesResponse
    {
        public List<FlatStatusDto> Statuses { get; set; } = new();
    }
}
