namespace SocietyLedger.Application.DTOs.Auth
{
    /// <summary>
    /// Response DTO returned after updating or fetching an authenticated user's profile.
    /// </summary>
    public class ProfileResponse
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Role { get; set; }
        public string? RoleDisplayName { get; set; }
    }
}
