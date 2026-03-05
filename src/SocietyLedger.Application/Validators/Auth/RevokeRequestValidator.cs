using FluentValidation;
using SocietyLedger.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Validators.Auth
{
    /// <summary>
    /// No body fields to validate — refresh token arrives via httpOnly cookie.
    /// </summary>
    public class RevokeRequestValidator : AbstractValidator<RevokeRequest>
    {
        public RevokeRequestValidator() { }
    }
}
