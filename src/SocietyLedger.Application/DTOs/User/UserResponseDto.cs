namespace SocietyLedger.Application.DTOs.User
{
    /// <summary>
    /// DTO for user response. Password hash is never included in responses.
    /// </summary>
    public record UserResponseDto(
        Guid PublicId,
        Guid SocietyPublicId,
        string Name,
        string? Email,
        string? Mobile,
        string? RoleDisplayName,
        bool IsActive,
        bool ForcePasswordChange,
        DateTime? LastLogin,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
