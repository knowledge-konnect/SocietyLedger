using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Infrastructure.Security
{
    public class TokenHasher
    {
        // Use SHA-256 to hash the refresh token before storing
        public static string Hash(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hashed = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashed); 
        }
    }
}
