using FluentValidation;
using SocietyLedger.Application.DTOs.Flat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Validators.Flat
{
    public class UpdateFlatDtoValidator : AbstractValidator<UpdateFlatDto>
    {
        public UpdateFlatDtoValidator()
        {
            RuleFor(x => x.PublicId)
                .NotEmpty()
                .WithMessage("PublicId is required for updating a flat.");

            RuleFor(x => x.FlatNo)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.FlatNo))
                .WithMessage("Flat number cannot exceed 50 characters.");

            RuleFor(x => x.OwnerName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.OwnerName))
                .WithMessage("Owner name cannot exceed 100 characters.");

            RuleFor(x => x.ContactMobile)
                .Matches(@"^[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactMobile))
                .WithMessage("Contact mobile must be a valid 10-digit number.");

            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
                .WithMessage("Contact email must be valid.");

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
