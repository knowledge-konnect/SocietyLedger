using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Application.DTOs.Plan;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class PlanEndpoints
    {
        public static void MapPlanRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Get all active plans
            app.MapGet("/",
            [SwaggerOperation(
                    Summary = "Get active plans",
                    Description = "Returns all active subscription plans available."
                )]
            async (IPlanService planService, HttpContext ctx) =>
                {
                    var result = await planService.GetActivePlansAsync();
                    return Results.Ok(ApiResponse<ListPlansResponse>.Success(
                        new ListPlansResponse { Plans = result.ToList() },
                        "Plans retrieved successfully"));
                })
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(version_1_0)
                .WithTags(groupName);

            // Get plan by ID
            app.MapGet("/{id}",
            
            [SwaggerOperation(
                    Summary = "Get plan by ID",
                    Description = "Returns a specific subscription plan by its ID."
                )]
            async (Guid id, IPlanService planService, HttpContext ctx) =>
                {
                    var result = await planService.GetPlanByIdAsync(id);
                    if (result == null)
                    {
                        var errorResponse = ErrorResponse.Create(ErrorCodes.RESOURCE_NOT_FOUND, "Plan not found", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 404);
                    }
                    return Results.Ok(ApiResponse<PlanResponse>.Success(result, "Plan retrieved successfully"));
                })
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(version_1_0)
                .WithTags(groupName);
        }
    }
}