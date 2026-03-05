using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Infrastructure.Security
{

    public static class RefreshTokenGenerator
    {
        // cryptographically secure random token string (not the hash)
        public static string Generate(int size = 64) // size bytes -> base64 => ~88 chars
        {
            var bytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(bytes);
        }
    }
}
