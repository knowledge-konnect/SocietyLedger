namespace SocietyLedger.Application.DTOs.User
{
    /// <summary>
    /// Slim response DTO for user creation.
    /// Password is never returned — admin always provides it explicitly.
    /// </summary>
    public record CreateUserResponseDto(
        Guid PublicId,
        string Email,
        string Username,
        string Message
    );
}
