using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.MaintenanceConfig;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class SocietyRoutes
    {
        public static void MapSocietyRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // GET /societies/{societyPublicId}/maintenance-config
            app.MapGet("/{societyPublicId:guid}/maintenance-config",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Get maintenance configuration",
                    Description = "Fetches the default maintenance billing configuration for a society. Admin or Treasurer role required."
                )]
                async (
                    Guid societyPublicId,
                    IMaintenanceConfigService configService,
                    HttpContext ctx) =>
                {
                    var userId = ctx.GetAuthenticatedUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized maintenance config get request - invalid user ID");
                        var err = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(err, statusCode: 401);
                    }

                    var config = await configService.GetAsync(societyPublicId, userId);
                    Log.Information("Maintenance config retrieved for society {SocietyPublicId} by user {UserId}", societyPublicId, userId);
                    return Results.Ok(ApiResponse<MaintenanceConfigResponse>.Success(config, "Maintenance configuration retrieved"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetMaintenanceConfig")
            .Produces<ApiResponse<MaintenanceConfigResponse>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);

            // PUT /societies/{societyPublicId}/maintenance-config
            app.MapPut("/{societyPublicId:guid}/maintenance-config",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Save maintenance configuration",
                    Description = "Creates or updates the maintenance billing configuration for a society (upsert). Admin or Treasurer role required."
                )]
                async (
                    Guid societyPublicId,
                    [FromBody] SaveMaintenanceConfigRequest request,
                    IMaintenanceConfigService configService,
                    HttpContext ctx) =>
                {
                    var userId = ctx.GetAuthenticatedUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized maintenance config save request - invalid user ID");
                        var err = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(err, statusCode: 401);
                    }

                    var config = await configService.SaveAsync(societyPublicId, request, userId);
                    Log.Information("Maintenance config saved for society {SocietyPublicId} by user {UserId}", societyPublicId, userId);
                    return Results.Ok(ApiResponse<MaintenanceConfigResponse>.Success(config, "Maintenance configuration saved successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<SaveMaintenanceConfigRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("SaveMaintenanceConfig")
            .Produces<ApiResponse<MaintenanceConfigResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);
        }
    }
}
