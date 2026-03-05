using SocietyLedger.Application.DTOs.Auth;
using SocietyLedger.Application.DTOs.User;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IUserService
    {
        /// <summary>Self-service: update the authenticated user's own profile (mobile only).</summary>
        Task<ProfileResponse> UpdateProfileAsync(long userId, UpdateProfileRequest request);
        Task<IEnumerable<UserResponseDto>> GetBySocietyAsync(long societyId);
        Task<UserResponseDto> GetByPublicIdAsync(Guid publicId, long societyId);
        Task<UserResponseDto?> GetUserByIdAsync(long userId);
        Task<CreateUserResponseDto> CreateAsync(CreateUserDto dto, long societyId);
        Task<UserResponseDto> UpdateAsync(UpdateUserDto dto, long societyId);
        Task DeleteByPublicIdAsync(Guid publicId, long societyId);


        Task<List<UserResponseDto>> GetUsersForAdminAsync(long authUserId);
        Task<UserResponseDto> GetUserByPublicIdForAdminAsync(Guid publicId, long authUserId);
        Task<CreateUserResponseDto> CreateUserForAdminAsync(CreateUserDto dto, long authUserId);
        Task<UserResponseDto> UpdateUserForAdminAsync(UpdateUserDto dto, long authUserId);
        Task<bool> DeleteUserForAdminAsync(Guid publicId, long authUserId);
    }
}
