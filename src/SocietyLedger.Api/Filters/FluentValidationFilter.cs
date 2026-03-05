using FluentValidation;
using SocietyLedger.Shared;

namespace SocietyLedger.Api.Filters
{
    /// <summary>
    /// Endpoint filter that runs FluentValidation for the typed request argument.
    /// Returns structured ErrorResponse with field-level errors on validation failure.
    /// </summary>
    public class FluentValidationFilter<TRequest> : IEndpointFilter where TRequest : class
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Try to find the typed request object from the endpoint arguments
            var model = context.Arguments.OfType<TRequest>().FirstOrDefault();
            if (model == null)
            {
                // Nothing to validate for this request type — continue
                return await next(context);
            }

            var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
            if (validator == null)
            {
                // No validator registered -> skip validation
                return await next(context);
            }

            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
                return await next(context);

            // Prepare consistent error payload with field-level errors
            var fieldErrors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .Select(g => new FieldError
                {
                    Field = g.Key,
                    Messages = g.Select(x => x.ErrorMessage).ToArray()
                })
                .ToList();

            var response = ErrorResponse.CreateWithFields(
                ErrorCodes.VALIDATION_FAILED,
                ErrorMessages.VALIDATION_FAILED,
                fieldErrors,
                context.HttpContext.TraceIdentifier
            );

            return Results.BadRequest(response);
        }
    }
}
