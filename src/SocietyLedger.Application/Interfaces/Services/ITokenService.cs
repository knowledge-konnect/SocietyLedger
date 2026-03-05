using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(long userId, IEnumerable<string> roles, out DateTime expiresAt);
        RefreshTokenPair GenerateRefreshToken();
    }


    public record RefreshTokenPair(string Token, DateTime ExpiresAt);
}
