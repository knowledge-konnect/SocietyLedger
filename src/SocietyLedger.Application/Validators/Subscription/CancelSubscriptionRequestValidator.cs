using FluentValidation;
using SocietyLedger.Application.DTOs.Subscription;

namespace SocietyLedger.Application.Validators.Subscription
{
    public class CancelSubscriptionRequestValidator : AbstractValidator<CancelSubscriptionRequest>
    {
        public CancelSubscriptionRequestValidator()
        {
            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}