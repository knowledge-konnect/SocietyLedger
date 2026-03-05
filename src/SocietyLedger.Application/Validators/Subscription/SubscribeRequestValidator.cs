using FluentValidation;
using SocietyLedger.Application.DTOs.Subscription;
using SocietyLedger.Domain.Constants;

namespace SocietyLedger.Application.Validators.Subscription
{
    public class SubscribeRequestValidator : AbstractValidator<SubscribeRequest>
    {
        public SubscribeRequestValidator()
        {
            RuleFor(x => x.PlanId)
                .NotEmpty().WithMessage("Plan ID is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .Must(method => new[] { PaymentModeCodes.Razorpay, PaymentModeCodes.BankTransfer, PaymentModeCodes.Upi, PaymentModeCodes.Cash, PaymentModeCodes.Cheque }.Contains(method.ToLower()))
                .WithMessage("Invalid payment method. Valid options: razorpay, bank_transfer, upi, cash, cheque");

            RuleFor(x => x.Amount)
                .GreaterThan(0).When(x => x.Amount.HasValue)
                .WithMessage("Amount must be greater than 0.");
        }
    }
}