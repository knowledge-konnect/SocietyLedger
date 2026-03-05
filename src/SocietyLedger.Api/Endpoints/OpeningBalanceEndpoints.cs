using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.OpeningBalance;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class OpeningBalanceEndpoints
    {
        public static void MapOpeningBalanceRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Get opening balance status
            app.MapGet("/status",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get opening balance status",
                    Description = "Checks if opening balance has been applied for the society and returns details."
                )]
            async (IOpeningBalanceService openingBalanceService,
                   IUserRepository userRepository,
                   HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();

                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized opening balance status request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid or missing authentication token", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    // Get user to extract societyId
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user == null || !user.IsActive)
                    {
                        Log.Warning("Opening balance status request by inactive or non-existent user {UserId}", userId);
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "User not found or inactive", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var societyId = user.SocietyId;
                    var status = await openingBalanceService.GetStatusAsync(societyId);

                    return Results.Ok(ApiResponse<OpeningBalanceStatusResponse>.Success(status, "Opening balance status retrieved"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetOpeningBalanceStatus")
            .Produces<ApiResponse<OpeningBalanceStatusResponse>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // Apply opening balance
            app.MapPost("/",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Apply opening balance",
                    Description = "Applies opening balance for a society. Can only be executed once per society. Restricted to Treasurer and Society Admin roles."
                )]
            async ([FromBody] OpeningBalanceRequest request, 
                   IOpeningBalanceService openingBalanceService, 
                   IUserRepository userRepository,
                   HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized opening balance request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid or missing authentication token", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    // Get user to extract societyId and check role
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user == null || !user.IsActive)
                    {
                        Log.Warning("Opening balance request by inactive or non-existent user {UserId}", userId);
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "User not found or inactive", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    // Check if user has Treasurer or Society Admin role
                    if (user.Role?.Code != RoleCodes.Treasurer && user.Role?.Code != RoleCodes.SocietyAdmin)
                    {
                        Log.Warning("Opening balance request by unauthorized user {UserId}, Role: {Role}", userId, user.Role?.Code);
                        var errorResponse = ErrorResponse.Create(ErrorCodes.FORBIDDEN, "Only Treasurer and Society Admin roles can apply opening balance", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 403);
                    }

                    var societyId = user.SocietyId;

                    await openingBalanceService.ApplyOpeningBalanceAsync(request, societyId, userId);
                    Log.Information("Opening balance applied successfully for society {SocietyId} by user {UserId}", societyId, userId);
                    return Results.Ok(ApiResponse<EmptyResponse>.Success(new EmptyResponse(), "Opening balance applied successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<OpeningBalanceRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("ApplyOpeningBalance")
            .Produces<ApiResponse<EmptyResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(500);
        }
    }
}
