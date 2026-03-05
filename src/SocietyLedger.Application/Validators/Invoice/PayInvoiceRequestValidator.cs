using FluentValidation;
using SocietyLedger.Application.DTOs.Invoice;
using SocietyLedger.Domain.Constants;

namespace SocietyLedger.Application.Validators.Invoice
{
    public class PayInvoiceRequestValidator : AbstractValidator<PayInvoiceRequest>
    {
        public PayInvoiceRequestValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .Must(method => new[] { PaymentModeCodes.Razorpay, PaymentModeCodes.BankTransfer, PaymentModeCodes.Upi, PaymentModeCodes.Cash, PaymentModeCodes.Cheque }.Contains(method.ToLower()))
                .WithMessage("Invalid payment method. Valid options: razorpay, bank_transfer, upi, cash, cheque");

            RuleFor(x => x.Amount)
                .GreaterThan(0).When(x => x.Amount.HasValue)
                .WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.PaymentReference)
                .MaximumLength(255).WithMessage("Payment reference cannot exceed 255 characters.");
        }
    }
}