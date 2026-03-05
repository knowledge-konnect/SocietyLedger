namespace SocietyLedger.Application.DTOs.User
{
    /// <summary>
    /// DTO for updating user details. Society context comes from authenticated user.
    /// </summary>
    public record UpdateUserDto(
        Guid PublicId,
        string Name,
        string Email,
        string? Mobile,
        string RoleCode
    );
}
