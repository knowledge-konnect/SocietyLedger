namespace SocietyLedger.Application.DTOs.User
{
    /// <summary>
    /// Paginated response for list users.
    /// </summary>
    public record PaginatedUsersResponseDto(
        IEnumerable<UserResponseDto> Users,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
