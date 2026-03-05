using Hangfire.Annotations;
using Hangfire.Dashboard;
using SocietyLedger.Domain.Constants;
using System.Security.Claims;

namespace SocietyLedger.Api.Authorization
{
    /// <summary>
    /// Restricts the Hangfire Dashboard to users who are authenticated and
    /// hold the <c>super_admin</c> or <c>admin</c> role.
    ///
    /// In production this filter is registered with <see cref="DashboardOptions.Authorization"/>.
    /// In development you may temporarily relax the check by setting
    /// <c>Hangfire:DashboardOpenInDevelopment = true</c> in appsettings.Development.json.
    /// </summary>
    [UsedImplicitly]
    public sealed class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public HangfireAuthorizationFilter(IWebHostEnvironment env, IConfiguration config)
        {
            _env    = env;
            _config = config;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow unrestricted access in Development if explicitly opted in via configuration.
            if (_env.IsDevelopment() &&
                _config.GetValue<bool>("Hangfire:DashboardOpenInDevelopment"))
            {
                return true;
            }

            // Require an authenticated user …
            if (httpContext.User.Identity?.IsAuthenticated != true)
                return false;

            // … with an admin-level role.
            return httpContext.User.HasClaim(
                       ClaimTypes.Role, RoleCodes.SuperAdmin) ||
                   httpContext.User.HasClaim(
                       ClaimTypes.Role, RoleCodes.Admin);
        }
    }
}
