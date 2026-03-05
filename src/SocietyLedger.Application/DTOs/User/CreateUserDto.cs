namespace SocietyLedger.Application.DTOs.User
{
    /// <summary>
    /// DTO for creating a new user. Society context comes from authenticated user.
    /// Password is always provided by the admin — no auto-generation.
    /// </summary>
    /// <summary>
    /// Username is required if email is not provided. Both must be unique.
    /// </summary>
    public record CreateUserDto(
        string Name,
        string? Email,
        string? Username,
        string? Mobile,
        string RoleCode,
        string Password
    );
}
