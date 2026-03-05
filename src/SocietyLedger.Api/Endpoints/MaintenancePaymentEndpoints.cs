using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.MaintenancePayment;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class MaintenancePaymentEndpoints
    {
        public static void MapMaintenancePaymentRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Create maintenance payment (idempotent, FIFO, transaction-safe)
            app.MapPost("/",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Create maintenance payment",
                    Description = """
                        Records a maintenance payment and allocates it FIFO to the oldest outstanding bills for the flat. Idempotency is enforced via the Idempotency-Key header (checked in maintenance_payments). Duplicate keys return the original result without writing new data. Entire allocation is transaction-safe. Any unallocated amount is reported as remainingAdvance.
                        """
                )]
            async ([FromBody] MaintenancePaymentRequest request, IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized maintenance payment create request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid or missing authentication token", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    if (request == null)
                    {
                        var errorResponse = ErrorResponse.Create(ErrorCodes.VALIDATION_FAILED, "Request body is required", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 400);
                    }
                    if (request.Amount <= 0)
                    {
                        var errorResponse = ErrorResponse.Create(ErrorCodes.VALIDATION_FAILED, "Amount must be positive", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                    // Resolve idempotency key: header → body → auto-generated
                    var idempotencyKey =
                        ctx.Request.Headers["Idempotency-Key"].FirstOrDefault()
                        ?? request.IdempotencyKey
                        ?? Guid.NewGuid().ToString();

                    var effectiveRequest = request with { IdempotencyKey = idempotencyKey };
                    try
                    {
                        var result = await paymentService.ProcessPaymentAsync(effectiveRequest, userId);
                        Log.Information(
                            "Maintenance payment processed: userId={UserId} flat={FlatId} totalPaid={TotalPaid} advance={Advance}",
                            userId, request.FlatPublicId, result.TotalPaid, result.RemainingAdvance);
                        return Results.Ok(ApiResponse<MaintenancePaymentResponse>.Success(result, "Maintenance payment processed successfully"));
                    }
                    catch (NotFoundException ex)
                    {
                        Log.Warning(ex, "Maintenance payment error: not found");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.RESOURCE_NOT_FOUND, ex.Message, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 404);
                    }
                    catch (ValidationException ex)
                    {
                        Log.Warning(ex, "Maintenance payment validation error");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.VALIDATION_FAILED, ex.Message, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 400);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Maintenance payment processing failed");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.INTERNAL_SERVER_ERROR, "An error occurred while processing the payment", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 500);
                    }
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("ProcessMaintenancePayment")
            .Produces<ApiResponse<MaintenancePaymentResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);

            // Get all maintenance payments for society
            app.MapGet("/",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get maintenance payments",
                    Description = "Retrieves all maintenance payments for the society."
                )]
            async (IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await paymentService.GetMaintenancePaymentsBySocietyAsync(userId);
                    return Results.Ok(ApiResponse<ListMaintenancePaymentsResponse>.Success(
                        new ListMaintenancePaymentsResponse(result.ToList()),
                        "Maintenance payments retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetMaintenancePayments")
            .Produces<ApiResponse<ListMaintenancePaymentsResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get maintenance payment by ID
            app.MapGet("/{publicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get maintenance payment by ID",
                    Description = "Retrieves a specific maintenance payment by its public ID."
                )]
            async (Guid publicId, IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await paymentService.GetMaintenancePaymentAsync(publicId, userId);
                    return Results.Ok(ApiResponse<MaintenancePaymentResponse>.Success(result, "Maintenance payment retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetMaintenancePaymentById")
            .Produces<ApiResponse<MaintenancePaymentResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get maintenance payments by flat
            app.MapGet("/flat/{flatPublicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get maintenance payments by flat",
                    Description = "Retrieves all maintenance payments for a specific flat."
                )]
            async (Guid flatPublicId, IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await paymentService.GetMaintenancePaymentsByFlatAsync(flatPublicId, userId);
                    return Results.Ok(ApiResponse<ListMaintenancePaymentsResponse>.Success(
                        new ListMaintenancePaymentsResponse(result.ToList()),
                        "Maintenance payments retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetMaintenancePaymentsByFlat")
            .Produces<ApiResponse<ListMaintenancePaymentsResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Update maintenance payment
            app.MapPut("/{publicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Update maintenance payment",
                    Description = "Updates an existing maintenance payment."
                )]
            async (Guid publicId, [FromBody] UpdateMaintenancePaymentRequest request, IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await paymentService.UpdateMaintenancePaymentAsync(publicId, userId, request);
                    return Results.Ok(ApiResponse<MaintenancePaymentResponse>.Success(result, "Maintenance payment updated successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<UpdateMaintenancePaymentRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("UpdateMaintenancePayment")
            .Produces<ApiResponse<MaintenancePaymentResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Delete maintenance payment
            app.MapDelete("/{publicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Delete maintenance payment",
                    Description = "Deletes a maintenance payment."
                )]
            async (Guid publicId, IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    await paymentService.DeleteMaintenancePaymentAsync(publicId, userId);
                    return Results.Ok(ApiResponse<EmptyResponse>.Success(null, "Maintenance payment deleted successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("DeleteMaintenancePayment")
            .Produces<ApiResponse<EmptyResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get maintenance summary for period
            app.MapGet("/summary",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get maintenance summary",
                    Description = "Retrieves maintenance charges and collection summary for a given period."
                )]
            async ([FromQuery] string period, IMaintenancePaymentService paymentService, IUserRepository userRepository, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized maintenance summary request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid or missing authentication token", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    if (string.IsNullOrEmpty(period))
                    {
                        Log.Warning("Maintenance summary request with missing period parameter");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.VALIDATION_FAILED, "Period parameter is required (format: yyyy-MM)", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                    var user = await userRepository.GetByIdAsync(userId);
                    if (user == null)
                    {
                        Log.Warning("Maintenance summary request by non-existent user {UserId}", userId);
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "User not found", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var summary = await paymentService.GetMaintenanceSummaryAsync(user.SocietyId, period);
                    Log.Information("Maintenance summary retrieved for society {SocietyId}, period {Period}", user.SocietyId, period);
                    return Results.Ok(ApiResponse<MaintenanceSummaryResponse>.Success(summary, "Maintenance summary retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetMaintenanceSummary")
            .Produces<ApiResponse<MaintenanceSummaryResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);
        }
    }
}
