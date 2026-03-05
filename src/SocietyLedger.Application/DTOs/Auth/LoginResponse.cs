using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SocietyLedger.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAt { get; set; }

        /// <summary>
        /// Not serialised — sent as an httpOnly cookie instead.
        /// </summary>
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Not serialised — the cookie expiry carries this information.
        /// </summary>
        [JsonIgnore]
        public DateTime RefreshTokenExpiresAt { get; set; }

        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public Guid UserPublicId { get; set; }
        public Guid SocietyPublicId { get; set; }
        public string SocietyName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public bool ForcePasswordChange { get; set; }
    }
}
