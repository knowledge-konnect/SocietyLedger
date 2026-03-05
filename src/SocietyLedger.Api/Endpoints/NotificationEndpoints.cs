using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Application.DTOs.Notification;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class NotificationRoutes
    {
        public static void MapNotificationRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // GET /notifications/preferences
            app.MapGet("/preferences",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Get notification preferences",
                    Description = "Returns the notification preferences for the authenticated user."
                )]
                async (INotificationPreferenceService prefService, HttpContext ctx) =>
                {
                    var userId = ctx.GetAuthenticatedUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized notification preferences get - invalid user ID");
                        var err = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(err, statusCode: 401);
                    }

                    var prefs = await prefService.GetPreferencesAsync(userId);
                    Log.Information("Notification preferences retrieved for user {UserId}", userId);
                    return Results.Ok(ApiResponse<NotificationPreferenceResponse>.Success(prefs, "Notification preferences retrieved"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetNotificationPreferences")
            .Produces<ApiResponse<NotificationPreferenceResponse>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // PUT /notifications/preferences
            app.MapPut("/preferences",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Update notification preferences",
                    Description = "Creates or updates notification preferences for the authenticated user. All fields are optional (partial update)."
                )]
                async (
                    [FromBody] UpdateNotificationPreferencesRequest request,
                    INotificationPreferenceService prefService,
                    HttpContext ctx) =>
                {
                    var userId = ctx.GetAuthenticatedUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized notification preferences update - invalid user ID");
                        var err = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(err, statusCode: 401);
                    }

                    var prefs = await prefService.UpdatePreferencesAsync(userId, request);
                    Log.Information("Notification preferences updated for user {UserId}", userId);
                    return Results.Ok(ApiResponse<NotificationPreferenceResponse>.Success(prefs, "Notification preferences updated successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("UpdateNotificationPreferences")
            .Produces<ApiResponse<NotificationPreferenceResponse>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);
        }
    }
}
