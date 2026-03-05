using FluentValidation;
using SocietyLedger.Application.DTOs.Auth;

namespace SocietyLedger.Application.Validators.Auth
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(6)
                .WithMessage("New password must be at least 6 characters.")
                .MaximumLength(128)
                .WithMessage("New password cannot exceed 128 characters.")
                .Must(x => HasValidPasswordStrength(x))
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword)
                .WithMessage("New password and confirm password do not match.");

            RuleFor(x => x)
                .Must(x => !x.CurrentPassword.Equals(x.NewPassword, StringComparison.Ordinal))
                .WithMessage("New password cannot be the same as current password.")
                .WithName("NewPassword");
        }

        private static bool HasValidPasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            bool hasUpperCase = password.Any(c => char.IsUpper(c));
            bool hasLowerCase = password.Any(c => char.IsLower(c));
            bool hasNumber = password.Any(c => char.IsDigit(c));

            return hasUpperCase && hasLowerCase && hasNumber;
        }
    }
}
