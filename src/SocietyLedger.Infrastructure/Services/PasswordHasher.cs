using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Infrastructure.Services
{
    // A simple, secure PBKDF2 hasher wrapper
    public class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 120_000; // strong


        public string Hash(string password)
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }


            var key = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, Iterations, KeySize);
            var result = new byte[1 + SaltSize + KeySize];
            result[0] = 0; // format marker
            Buffer.BlockCopy(salt, 0, result, 1, SaltSize);
            Buffer.BlockCopy(key, 0, result, 1 + SaltSize, KeySize);
            return Convert.ToBase64String(result);
        }


        public bool Verify(string hash, string password)
        {
            var bytes = Convert.FromBase64String(hash);
            if (bytes[0] != 0) return false;
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(bytes, 1, salt, 0, SaltSize);
            var key = new byte[KeySize];
            Buffer.BlockCopy(bytes, 1 + SaltSize, key, 0, KeySize);
            var incoming = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, Iterations, KeySize);
            return CryptographicOperations.FixedTimeEquals(incoming, key);
        }
    }
}
