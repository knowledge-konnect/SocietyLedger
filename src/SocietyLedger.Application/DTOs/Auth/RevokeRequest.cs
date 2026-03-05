using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.DTOs.Auth
{
    /// <summary>
    /// POST /auth/revoke — refresh token is read from the httpOnly cookie; no body required.
    /// </summary>
    public record RevokeRequest();
}
