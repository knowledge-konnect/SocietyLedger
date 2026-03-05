using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocietyLedger.Application.Interfaces;
using SocietyLedger.Shared.Jwt;
using SocietyLedger.Infrastructure.Security;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Security
{
    public static class PasswordHasher
    {
        public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public static bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
