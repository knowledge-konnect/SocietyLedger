using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocietyLedger.Shared.Jwt
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(int userId, string email, string role);
        (string token, DateTime expiresAt, string jti) CreateAccessTokenWithJti(int userId, string email, string role);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _settings;
        private readonly byte[] _secretBytes;

        public JwtTokenService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
            _secretBytes = Encoding.UTF8.GetBytes(_settings.Key);
        }

        public string CreateAccessToken(int userId, string email, string role)
        {
            var (token, _, _) = CreateAccessTokenWithJti(userId, email, role);
            return token;
        }

        public (string token, DateTime expiresAt, string jti) CreateAccessTokenWithJti(int userId, string email, string role)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_settings.AccessTokenExpirationMinutes);

            var jti = Guid.NewGuid().ToString(); // unique id for token
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

            var signingKey = new SymmetricSecurityKey(_secretBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer != string.Empty ? _settings.Issuer : null,
                audience: _settings.Audience != string.Empty ? _settings.Audience : null,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenStr, expires, jti);
        }
    }
}
