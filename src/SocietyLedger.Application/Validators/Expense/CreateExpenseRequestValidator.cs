using FluentValidation;
using SocietyLedger.Application.DTOs.Expense;

namespace SocietyLedger.Application.Validators.Expense
{
    public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
    {
        public CreateExpenseRequestValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Expense date is required.")
                // Upper-bound checked here; lower-bound (>= onboarding_date) requires a DB
                // lookup and is enforced in the service layer.
                .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("Expense date cannot be in the future.");

            RuleFor(x => x.CategoryCode)
                .NotEmpty().WithMessage("Category code is required.")
                .MaximumLength(50).WithMessage("Category code cannot exceed 50 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Vendor)
                .MaximumLength(200).WithMessage("Vendor name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Vendor));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
