using Serilog;
using System.Security.Claims;

namespace SocietyLedger.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            Log.Information(
                "HTTP {Method} {Path} started. UserId: {UserId}",
                context.Request.Method,
                context.Request.Path,
                userId
            );

            await _next(context);

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Log.Information(
                "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms. UserId: {UserId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration,
                userId
            );
        }
    }
}
