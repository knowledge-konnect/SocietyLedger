using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.Subscription;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class SubscriptionEndpoints
    {
        public static void MapSubscriptionRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Create trial subscription
            app.MapPost("/trial",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Create trial subscription",
                    Description = "Creates a 30-day free trial for the authenticated user on first registration."
                )]
            async (ISubscriptionService subscriptionService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    await subscriptionService.CreateTrialSubscriptionAsync(userId);
                    // Assuming trial end can be retrieved or calculated; adjust if service returns it
                    var status = await subscriptionService.GetSubscriptionStatusAsync(userId);
                    return Results.Ok(ApiResponse<SubscriptionStatusResponse>.Success(status, "Trial started"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("CreateTrialSubscription")
            .Produces<ApiResponse<SubscriptionStatusResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get subscription status
            app.MapGet("/status",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get subscription status",
                    Description = "Returns the current subscription status for the authenticated user."
                )]
            async (ISubscriptionService subscriptionService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await subscriptionService.GetSubscriptionStatusAsync(userId);
                    return Results.Ok(ApiResponse<SubscriptionStatusResponse>.Success(result, "Subscription status retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetSubscriptionStatus")
            .Produces<ApiResponse<SubscriptionStatusResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Subscribe to a plan
            app.MapPost("/subscribe",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Subscribe to a plan",
                    Description = "Creates a subscription and processes payment."
                )]
            async ([FromBody] SubscribeRequest request, ISubscriptionService subscriptionService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await subscriptionService.SubscribeAsync(userId, request);
                    return Results.Ok(ApiResponse<SubscribeResponse>.Success(result, "Subscription created successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<SubscribeRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("Subscribe")
            .Produces<ApiResponse<SubscribeResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Cancel subscription
            app.MapPost("/cancel",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Cancel subscription",
                    Description = "Cancels the user's active subscription."
                )]
            async ([FromBody] CancelSubscriptionRequest request, ISubscriptionService subscriptionService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    await subscriptionService.CancelSubscriptionAsync(userId, request);
                    return Results.Ok(ApiResponse<EmptyResponse>.Success(null, "Subscription cancelled successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<CancelSubscriptionRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("CancelSubscription")
            .Produces<ApiResponse<EmptyResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);
        }
    }
}