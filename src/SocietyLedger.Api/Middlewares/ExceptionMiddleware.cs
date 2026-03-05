using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using System.Net;
using System.Text.Json;

namespace SocietyLedger.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = context.TraceIdentifier;
                if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var headerCorrelationId))
                {
                    correlationId = headerCorrelationId!;
                }
                _logger.LogError(ex, "Exception: {Message}. CorrelationId: {CorrelationId}", ex.Message, correlationId);
                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            var (statusCode, response) = exception switch
            {
                ValidationException vex => (
                    HttpStatusCode.BadRequest,
                    ErrorResponse.CreateWithFields(
                        vex.Code,
                        vex.Message,
                        vex.Errors.Select(e => new FieldError { Field = e.Key, Messages = e.Value }).ToList(),
                        correlationId
                    )
                ),
                AuthenticationException aex => (
                    HttpStatusCode.Unauthorized,
                    ErrorResponse.Create(aex.Code, aex.Message, correlationId)
                ),
                AuthorizationException azex => (
                    HttpStatusCode.Forbidden,
                    ErrorResponse.Create(azex.Code, azex.Message, correlationId)
                ),
                NotFoundException nex => (
                    HttpStatusCode.NotFound,
                    ErrorResponse.Create(nex.Code, nex.Message, correlationId)
                ),
                ConflictException cex => (
                    HttpStatusCode.Conflict,
                    ErrorResponse.Create(cex.Code, cex.Message, correlationId)
                ),
                DuplicateException dex => (
                    HttpStatusCode.Conflict,
                    ErrorResponse.Create(dex.Code, dex.Message, correlationId)
                ),
                AppException aex => (
                    HttpStatusCode.BadRequest,
                    ErrorResponse.Create(aex.Code, aex.Message, correlationId)
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    ErrorResponse.Create(
                        ErrorCodes.INTERNAL_SERVER_ERROR,
                        ErrorMessages.INTERNAL_SERVER_ERROR,
                        correlationId
                    )
                )
            };

            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }
}
