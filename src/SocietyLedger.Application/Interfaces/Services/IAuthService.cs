using SocietyLedger.Application.DTOs.Auth;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, string ipAddress);
        Task<LoginResponse> RefreshTokenAsync(string token, string ipAddress);
        Task RevokeRefreshTokenAsync(string token, string ipAddress);
        Task<ChangePasswordResponse> ChangePasswordAsync(long userId, ChangePasswordRequest request);
    }
}
