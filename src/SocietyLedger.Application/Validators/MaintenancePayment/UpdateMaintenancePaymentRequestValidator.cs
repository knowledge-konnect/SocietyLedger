using FluentValidation;
using SocietyLedger.Application.DTOs.MaintenancePayment;

namespace SocietyLedger.Application.Validators.MaintenancePayment
{
    public class UpdateMaintenancePaymentRequestValidator : AbstractValidator<UpdateMaintenancePaymentRequest>
    {
        public UpdateMaintenancePaymentRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).When(x => x.Amount.HasValue)
                .WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.PaymentDate)
                // Use a lambda so DateTime.UtcNow is evaluated per-request, not once at
                // class construction time (a common FluentValidation gotcha).
                .LessThanOrEqualTo(_ => DateTime.UtcNow)
                    .WithMessage("Payment date cannot be in the future.")
                .When(x => x.PaymentDate.HasValue);

            RuleFor(x => x.PaymentModeCode)
                .NotEmpty().When(x => x.PaymentModeCode != null)
                .WithMessage("Payment mode code cannot be empty when provided.")
                .MaximumLength(50).When(x => x.PaymentModeCode != null)
                .WithMessage("Payment mode code cannot exceed 50 characters.");

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(255).WithMessage("Reference number cannot exceed 255 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
