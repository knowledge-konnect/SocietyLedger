using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Shared.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SocietyLedger.Infrastructure.Services
{

    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();


        public TokenService(IOptions<JwtSettings> options) { _settings = options.Value; }


        public string GenerateAccessToken(long userId, IEnumerable<string> roles, out DateTime expiresAt)
        {
            var claims = new List<Claim>
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };


            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
            var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public RefreshTokenPair GenerateRefreshToken()
        {
            var tokenBytes = new byte[64];
            _rng.GetBytes(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes);
            var expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
            return new RefreshTokenPair(token, expires);
        }
    }
}
