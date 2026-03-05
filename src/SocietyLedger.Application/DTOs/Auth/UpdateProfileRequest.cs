namespace SocietyLedger.Application.DTOs.Auth
{
    /// <summary>
    /// Request DTO for self-service profile update (only mobile is updatable via this endpoint).
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>Optional. 10-digit numeric mobile number.</summary>
        public string? Mobile { get; set; }
    }
}
