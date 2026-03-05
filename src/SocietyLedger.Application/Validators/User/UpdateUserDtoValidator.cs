using FluentValidation;
using SocietyLedger.Application.DTOs.User;

namespace SocietyLedger.Application.Validators.User
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.PublicId)
                .NotEmpty()
                .WithMessage("PublicId is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("User name is required.")
                .MaximumLength(100)
                .WithMessage("User name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid email address.")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters.");

            RuleFor(x => x.Mobile)
                .Matches(@"^[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Mobile))
                .WithMessage("Mobile must be a valid 10-digit number.");

            RuleFor(x => x.RoleCode)
                .NotEmpty()
                .WithMessage("Role code is required.")
                .MaximumLength(50)
                .WithMessage("Role code cannot exceed 50 characters.");
        }
    }
}
