namespace SocietyLedger.Application.DTOs.Flat
{
    /// <summary>
    /// Request body for the PUT /flats/{publicId}/opening-balance endpoint.
    /// Allows an admin or treasurer to set the pre-system outstanding balance
    /// for a specific flat at onboarding time (or to correct it later).
    /// </summary>
    public record SetOpeningBalanceRequest(decimal Amount);
}
