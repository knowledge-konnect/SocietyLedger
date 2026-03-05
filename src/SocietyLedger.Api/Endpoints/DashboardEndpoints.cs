using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Application.DTOs.Dashboard;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Services;
using SocietyLedger.Shared;

namespace SocietyLedger.Api.Endpoints
{
    public static class DashboardEndpoints
    {
        public static void MapDashboardEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/dashboard")
                .WithName("Dashboard")
                .RequireAuthorization()
                .WithTags("Dashboard");

            group.MapGet("/", GetDashboard)
                .WithName("GetDashboard")
                .WithDescription("Get complete dashboard data for authenticated user's society")
                .Produces<ApiResponse<DashboardResponseDto>>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        private static async Task<IResult> GetDashboard(
            IDashboardService dashboardService,
            IUserRepository userRepository,
            HttpContext httpContext,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = httpContext.GetUserId();
                if (userId == 0)
                {
                    Log.Warning("Unauthorized dashboard request - invalid user ID");
                    var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid or missing authentication token", httpContext.TraceIdentifier);
                    return Results.Json(errorResponse, statusCode: 401);
                }

                // Get user to extract societyId
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    Log.Warning("Dashboard request by inactive or non-existent user {UserId}", userId);
                    var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "User not found or inactive", httpContext.TraceIdentifier);
                    return Results.Json(errorResponse, statusCode: 401);
                }

                // Validate dates if provided
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                    return Results.BadRequest(
                        ApiResponse<object>.Fail("Start date must be before end date"));

                // Get dashboard data
                var dashboardData = await dashboardService.GetDashboardDataAsync(
                    user.SocietyId, startDate, endDate, cancellationToken);

                return Results.Ok(
                    ApiResponse<DashboardResponseDto>.Success(
                        dashboardData,
                        "Dashboard data retrieved successfully"));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(
                    ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception)
            {
                return Results.StatusCode(
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}
