using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SocietyLedger.Api.Authorization
{
    public class SubscriptionAuthorizationHandler : AuthorizationHandler<SubscriptionRequirement>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubscriptionAuthorizationHandler(
            ISubscriptionService subscriptionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _subscriptionService = subscriptionService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SubscriptionRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            // Get user ID from JWT token claims (using standard 'sub' claim)
            var userIdClaim =
      context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
      ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                context.Fail();
                return;
            }


            try
            {
                // Check subscription status
                var subscriptionStatus = await _subscriptionService.GetSubscriptionStatusAsync(userId);

                // Allow access if subscription is active or trial is still valid
                if (subscriptionStatus.Status == SubscriptionStatusCodes.Active ||
                    (subscriptionStatus.Status == SubscriptionStatusCodes.Trial && subscriptionStatus.TrialEndDate > DateTime.UtcNow))
                {
                    context.Succeed(requirement);
                    return;
                }

                // Deny access for expired trials or other statuses
                context.Fail();
            }
            catch (Exception)
            {
                // If there's an error checking subscription, deny access for security
                context.Fail();
            }
        }
    }
}