namespace SocietyLedger.Application.DTOs.User
{
    /// <summary>
    /// Wrapper kept for backwards compatibility; the endpoint returns CreateUserResponseDto directly.
    /// </summary>
    public class CreateUserResponse
    {
        public Guid PublicId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
