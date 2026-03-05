using FluentValidation;
using SocietyLedger.Application.DTOs.Auth;

namespace SocietyLedger.Application.Validators.Auth
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            // Mobile is optional; only validate if provided
            RuleFor(x => x.Mobile)
                .Matches(@"^[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Mobile))
                .WithMessage("Mobile must be exactly 10 digits and numeric only.");
        }
    }
}
