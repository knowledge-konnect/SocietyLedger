using FluentValidation;
using SocietyLedger.Application.DTOs.MaintenancePayment;

namespace SocietyLedger.Application.Validators.MaintenancePayment
{
    public class CreateMaintenancePaymentRequestValidator : AbstractValidator<CreateMaintenancePaymentRequest>
    {
        public CreateMaintenancePaymentRequestValidator()
        {
            RuleFor(x => x.FlatPublicId)
                .NotEmpty().WithMessage("FlatPublicId is required.")
                .Must(id => id != Guid.Empty).WithMessage("FlatPublicId must be a valid GUID.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("Payment date is required.")
                // Upper-bound checked here; lower-bound (>= onboarding_date) requires a DB
                // lookup and is enforced in the service layer.
                .LessThanOrEqualTo(_ => DateTime.UtcNow)
                    .WithMessage("Payment date cannot be in the future.");

            RuleFor(x => x.PaymentModeCode)
                .NotEmpty().WithMessage("Payment mode code is required.")
                .MaximumLength(50).WithMessage("Payment mode code cannot exceed 50 characters.");

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(100).WithMessage("Reference number cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.ReferenceNumber));

            RuleFor(x => x.ReceiptUrl)
                .MaximumLength(500).WithMessage("Receipt URL cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.ReceiptUrl));

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
