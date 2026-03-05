using System.Security.Claims;

namespace SocietyLedger.Api.Extensions
{
    /// <summary>
    /// Provides helper extensions for working with HttpContext.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Safely retrieves the real client IP address,
        /// normalizing IPv6 loopback (::1) to IPv4 (127.0.0.1),
        /// and handling X-Forwarded-For headers.
        /// </summary>
        public static string GetClientIp(this HttpContext ctx)
        {
            if (ctx == null)
                return "unknown";

            try
            {
                string ip = null;

                // Check X-Forwarded-For (proxy headers)
                var forwarded = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwarded))
                {
                    // May contain multiple IPs (client, proxy1, proxy2)
                    ip = forwarded.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .FirstOrDefault()?.Trim();
                }

                // If still not found, use direct connection IP
                ip ??= ctx.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrWhiteSpace(ip))
                    return "unknown";

                // Normalize IPv6 localhost (::1) to IPv4
                if (ip == "::1" || ip == "0:0:0:0:0:0:0:1")
                    ip = "127.0.0.1";

                // Optional: Remove port if somehow included
                if (ip.Contains(':') && System.Net.IPAddress.TryParse(ip, out var parsedIp))
                {
                    // For mapped IPv6-to-IPv4 (like ::ffff:192.168.1.10)
                    if (parsedIp.IsIPv4MappedToIPv6)
                        ip = parsedIp.MapToIPv4().ToString();
                }

                return ip;
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// Gets the authenticated user's ID from JWT claims.
        /// </summary>
        public static long GetAuthenticatedUserId(this HttpContext ctx)
        {
            if (ctx?.User == null)
                return 0;

            var userIdClaim =
                ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? ctx.User.FindFirst("sub")?.Value;

            return long.TryParse(userIdClaim, out var userId)
                ? userId
                : 0;
        }

        /// <summary>
        /// Gets the authenticated user's ID from JWT claims (alias for GetAuthenticatedUserId).
        /// </summary>
        public static long GetUserId(this HttpContext ctx)
        {
            return ctx.GetAuthenticatedUserId();
        }
    }
}
