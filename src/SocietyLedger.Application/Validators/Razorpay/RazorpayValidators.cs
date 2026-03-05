using FluentValidation;
using SocietyLedger.Application.DTOs.Razorpay;

namespace SocietyLedger.Application.Validators.Razorpay
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.PlanId)
                .NotEmpty()
                .WithMessage("PlanId is required.");
        }
    }

    public class VerifyPaymentRequestValidator : AbstractValidator<VerifyPaymentRequest>
    {
        public VerifyPaymentRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("OrderId is required.");

            RuleFor(x => x.PaymentId)
                .NotEmpty()
                .WithMessage("PaymentId is required.");

            RuleFor(x => x.Signature)
                .NotEmpty()
                .WithMessage("Signature is required.");
        }
    }
}