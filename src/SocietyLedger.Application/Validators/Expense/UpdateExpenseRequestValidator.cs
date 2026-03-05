using FluentValidation;
using SocietyLedger.Application.DTOs.Expense;

namespace SocietyLedger.Application.Validators.Expense
{
    public class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
    {
        public UpdateExpenseRequestValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Expense date is required.")
                .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("Expense date cannot be in the future.")
                .When(x => x.Date.HasValue);

            RuleFor(x => x.CategoryCode)
                .NotEmpty().WithMessage("Category code cannot be empty.")
                .MaximumLength(50).WithMessage("Category code cannot exceed 50 characters.")
                .When(x => x.CategoryCode != null);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.")
                .When(x => x.Amount.HasValue);

            RuleFor(x => x.Vendor)
                .MaximumLength(200).WithMessage("Vendor name cannot exceed 200 characters.")
                .When(x => x.Vendor != null);

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => x.Description != null);
        }
    }
}
