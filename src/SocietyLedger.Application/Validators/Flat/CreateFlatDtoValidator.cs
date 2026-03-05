using FluentValidation;
using SocietyLedger.Application.DTOs.Flat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Validators.Flat
{
    public class CreateFlatDtoValidator : AbstractValidator<CreateFlatDto>
    {
        public CreateFlatDtoValidator()
        {
            RuleFor(x => x.FlatNo)
                .NotEmpty()
                .WithMessage("Flat number is required.")
                .MaximumLength(50)
                .WithMessage("Flat number cannot exceed 50 characters.");

            RuleFor(x => x.OwnerName)
                .NotEmpty()
                .WithMessage("Owner name is required.")
                .MaximumLength(100)
                .WithMessage("Owner name cannot exceed 100 characters.");

            RuleFor(x => x.ContactMobile)
                .NotEmpty()
                .WithMessage("Contact mobile number is required.")
                .Matches(@"^[0-9]{10}$")
                .WithMessage("Contact mobile must be a valid 10-digit number.");

            //RuleFor(x => x.ContactEmail)
            //    .NotEmpty()
            //    .WithMessage("Contact email is required.")
            //    .EmailAddress()
            //    .WithMessage("Contact email must be a valid email address.");

            RuleFor(x => x.MaintenanceAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaintenanceAmount.HasValue)
                .WithMessage("Maintenance amount cannot be negative.");

            RuleFor(x => x.StatusCode)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.StatusCode))
                .WithMessage("Status code cannot exceed 20 characters.");
        }
    }
}
