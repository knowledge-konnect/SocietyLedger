using FluentValidation;
using SocietyLedger.Application.DTOs.Billing;

namespace SocietyLedger.Application.Validators.Billing
{
    public class GenerateBillsRequestValidator : AbstractValidator<GenerateBillsRequest>
    {
        public GenerateBillsRequestValidator()
        {
            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Period is required.")
                .Matches(@"^\d{4}-\d{2}$").WithMessage("Period must be in YYYY-MM format (e.g. '2026-03').")
                .Must(BeAValidYearMonth).WithMessage("Period must be a valid year and month.");
        }

        private static bool BeAValidYearMonth(string period)
        {
            if (string.IsNullOrWhiteSpace(period)) return false;
            var parts = period.Split('-');
            return parts.Length == 2
                && int.TryParse(parts[0], out int year) && year is >= 2000 and <= 2100
                && int.TryParse(parts[1], out int month) && month is >= 1 and <= 12;
        }
    }
}
