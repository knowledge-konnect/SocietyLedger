namespace SocietyLedger.Application.DTOs.User
{
    public class ListUsersResponse
    {
        public List<UserResponseDto> Users { get; set; } = new();
    }
}
