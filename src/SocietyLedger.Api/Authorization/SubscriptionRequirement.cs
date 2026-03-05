using Microsoft.AspNetCore.Authorization;

namespace SocietyLedger.Api.Authorization
{
    public class SubscriptionRequirement : IAuthorizationRequirement
    {
        // This requirement ensures the user has an active subscription or valid trial
    }
}