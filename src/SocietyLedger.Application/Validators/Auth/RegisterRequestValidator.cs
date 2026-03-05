using FluentValidation;
using SocietyLedger.Application.DTOs.Auth;

namespace SocietyLedger.Application.Validators.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.")
                .MaximumLength(200);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.SocietyName)
                .NotEmpty().WithMessage("Society name is required.")
                .MaximumLength(250);

            RuleFor(x => x.SocietyAddress)
                .NotEmpty().WithMessage("Society address is required.")
                .MaximumLength(500);
        }
    }
}
