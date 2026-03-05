using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Application.DTOs.Reports;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class ReportEndpoints
    {
        public static void MapReportRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var v1 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // ------------------------------------------------------------------ //
            // 1. Collection Summary                                                //
            // ------------------------------------------------------------------ //
            app.MapGet("/collection-summary",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Collection Summary",
                    Description = "Total billed vs collected vs outstanding per period. Optionally filter by period range (format: YYYY-MM)."
                )]
                async (
                    IReportService reportService,
                    HttpContext ctx,
                    [FromQuery] string? startPeriod,
                    [FromQuery] string? endPeriod,
                    CancellationToken ct) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0) return Results.Json(
                        ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid token", ctx.TraceIdentifier), statusCode: 401);

                    var result = await reportService.GetCollectionSummaryAsync(userId, startPeriod, endPeriod, ct);
                    return Results.Ok(ApiResponse<CollectionSummaryDto>.Success(result, "Collection summary retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet).HasApiVersion(v1)
            .WithName("GetCollectionSummary")
            .Produces<ApiResponse<CollectionSummaryDto>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // ------------------------------------------------------------------ //
            // 2. Defaulters Report                                                 //
            // ------------------------------------------------------------------ //
            app.MapGet("/defaulters",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Defaulters Report",
                    Description = "Lists all flats with pending dues sorted by outstanding amount. Use minOutstanding to filter (default: 0)."
                )]
                async (
                    IReportService reportService,
                    HttpContext ctx,
                    [FromQuery] decimal minOutstanding = 0,
                    CancellationToken ct = default) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0) return Results.Json(
                        ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid token", ctx.TraceIdentifier), statusCode: 401);

                    var result = await reportService.GetDefaultersReportAsync(userId, minOutstanding, ct);
                    return Results.Ok(ApiResponse<List<DefaulterDto>>.Success(result, "Defaulters report retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet).HasApiVersion(v1)
            .WithName("GetDefaultersReport")
            .Produces<ApiResponse<List<DefaulterDto>>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // ------------------------------------------------------------------ //
            // 3. Income vs Expense                                                 //
            // ------------------------------------------------------------------ //
            app.MapGet("/income-vs-expense",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Income vs Expense",
                    Description = "Monthly income (collections) vs expenses with net surplus/deficit. Filter by date range."
                )]
                async (
                    IReportService reportService,
                    HttpContext ctx,
                    [FromQuery] DateOnly? startDate,
                    [FromQuery] DateOnly? endDate,
                    CancellationToken ct) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0) return Results.Json(
                        ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid token", ctx.TraceIdentifier), statusCode: 401);

                    var result = await reportService.GetIncomeVsExpenseAsync(userId, startDate, endDate, ct);
                    return Results.Ok(ApiResponse<IncomeVsExpenseDto>.Success(result, "Income vs expense report retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet).HasApiVersion(v1)
            .WithName("GetIncomeVsExpense")
            .Produces<ApiResponse<IncomeVsExpenseDto>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // ------------------------------------------------------------------ //
            // 4. Fund Ledger                                                       //
            // ------------------------------------------------------------------ //
            app.MapGet("/fund-ledger",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Society Fund Ledger",
                    Description = "Full transaction history of the society fund with running balance. Filter by date range."
                )]
                async (
                    IReportService reportService,
                    HttpContext ctx,
                    [FromQuery] DateOnly? startDate,
                    [FromQuery] DateOnly? endDate,
                    CancellationToken ct) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0) return Results.Json(
                        ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid token", ctx.TraceIdentifier), statusCode: 401);

                    var result = await reportService.GetFundLedgerAsync(userId, startDate, endDate, ct);
                    return Results.Ok(ApiResponse<FundLedgerReportDto>.Success(result, "Fund ledger retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet).HasApiVersion(v1)
            .WithName("GetFundLedger")
            .Produces<ApiResponse<FundLedgerReportDto>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // ------------------------------------------------------------------ //
            // 5. Payment Collection Register                                       //
            // ------------------------------------------------------------------ //
            app.MapGet("/payment-register",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Payment Collection Register",
                    Description = "Paginated list of all payments received — flat, owner, amount, mode and reference. " +
                                  "Filter by date range. Use 'page' and 'pageSize' for pagination (defaults: 1 / 50)."
                )]
                async (
                    IReportService reportService,
                    HttpContext ctx,
                    [FromQuery] DateOnly? startDate,
                    [FromQuery] DateOnly? endDate,
                    [FromQuery] int page     = 1,
                    [FromQuery] int pageSize = 50,
                    CancellationToken ct = default) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0) return Results.Json(
                        ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid token", ctx.TraceIdentifier), statusCode: 401);

                    var result = await reportService.GetPaymentRegisterAsync(userId, startDate, endDate, page, pageSize, ct);
                    return Results.Ok(ApiResponse<PagedResult<PaymentRegisterDto>>.Success(result, "Payment register retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet).HasApiVersion(v1)
            .WithName("GetPaymentRegister")
            .Produces<ApiResponse<PagedResult<PaymentRegisterDto>>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            // ------------------------------------------------------------------ //
            // 6. Expense by Category                                               //
            // ------------------------------------------------------------------ //
            app.MapGet("/expense-by-category",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Expense by Category",
                    Description = "Total spending broken down by expense category with entry count and date range. Filter by date range."
                )]
                async (
                    IReportService reportService,
                    HttpContext ctx,
                    [FromQuery] DateOnly? startDate,
                    [FromQuery] DateOnly? endDate,
                    CancellationToken ct) =>
                {
                    var userId = ctx.GetUserId();
                    if (userId == 0) return Results.Json(
                        ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid token", ctx.TraceIdentifier), statusCode: 401);

                    var result = await reportService.GetExpenseByCategoryAsync(userId, startDate, endDate, ct);
                    return Results.Ok(ApiResponse<ExpenseByCategoryDto>.Success(result, "Expense by category retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet).HasApiVersion(v1)
            .WithName("GetExpenseByCategory")
            .Produces<ApiResponse<ExpenseByCategoryDto>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);
        }
    }
}
