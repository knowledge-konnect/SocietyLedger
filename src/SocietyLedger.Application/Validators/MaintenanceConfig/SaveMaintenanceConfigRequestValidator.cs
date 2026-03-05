using FluentValidation;
using SocietyLedger.Application.DTOs.MaintenanceConfig;

namespace SocietyLedger.Application.Validators.MaintenanceConfig
{
    public class SaveMaintenanceConfigRequestValidator : AbstractValidator<SaveMaintenanceConfigRequest>
    {
        public SaveMaintenanceConfigRequestValidator()
        {
            RuleFor(x => x.DefaultMonthlyCharge)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Default monthly charge must be 0 or greater.");

            RuleFor(x => x.DueDayOfMonth)
                .InclusiveBetween(1, 28)
                .WithMessage("Due day of month must be between 1 and 28.");

            RuleFor(x => x.LateFeePerMonth)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Late fee per month must be 0 or greater.");

            RuleFor(x => x.GracePeriodDays)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Grace period days must be 0 or greater.");
        }
    }
}
