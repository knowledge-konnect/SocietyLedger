using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.Expense;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class ExpenseEndpoints
    {
        public static void MapExpenseRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Create expense
            app.MapPost("/",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Create expense",
                    Description = "Records a new expense for the society."
                )]
            async ([FromBody] CreateExpenseRequest request, IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized expense create request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, "Invalid or missing authentication token", ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var result = await expenseService.CreateExpenseAsync(userId, request);
                    Log.Information("Expense created successfully by user {UserId}", userId);
                    return Results.Created($"/expenses/{result.PublicId}", ApiResponse<ExpenseResponse>.Success(result, "Expense created successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<CreateExpenseRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("CreateExpense")
            .Produces<ApiResponse<ExpenseResponse>>(201)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get all expenses for society
            app.MapGet("/",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get expenses",
                    Description = "Retrieves all expenses for the society."
                )]
            async (IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await expenseService.GetExpensesBySocietyAsync(userId);
                    return Results.Ok(ApiResponse<ListExpensesResponse>.Success(
                        new ListExpensesResponse { Expenses = result.ToList() },
                        "Expenses retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetExpenses")
            .Produces<ApiResponse<ListExpensesResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get expense by ID
            app.MapGet("/{publicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get expense by ID",
                    Description = "Retrieves a specific expense by its public ID."
                )]
            async (Guid publicId, IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await expenseService.GetExpenseAsync(publicId, userId);
                    return Results.Ok(ApiResponse<ExpenseResponse>.Success(result, "Expense retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetExpenseById")
            .Produces<ApiResponse<ExpenseResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get expenses by date range
            app.MapGet("/range",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get expenses by date range",
                    Description = "Retrieves expenses for a specific date range."
                )]
            async ([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate, IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await expenseService.GetExpensesByDateRangeAsync(userId, startDate, endDate);
                    return Results.Ok(ApiResponse<ListExpensesResponse>.Success(
                        new ListExpensesResponse { Expenses = result.ToList() },
                        "Expenses retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetExpensesByDateRange")
            .Produces<ApiResponse<ListExpensesResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get expense categories
            app.MapGet("/categories",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get expense categories",
                    Description = "Retrieves all available expense categories."
                )]
            async (IExpenseService expenseService, HttpContext ctx) =>
                {
                    var result = await expenseService.GetExpenseCategoriesAsync();
                    return Results.Ok(ApiResponse<ExpenseCategoriesResponse>.Success(
                        new ExpenseCategoriesResponse { Categories = result.ToList() },
                        "Expense categories retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetExpenseCategories")
            .Produces<ApiResponse<ExpenseCategoriesResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Get expenses by category
            app.MapGet("/category/{categoryCode}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get expenses by category",
                    Description = "Retrieves all expenses for a specific category."
                )]
            async (string categoryCode, IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await expenseService.GetExpensesByCategoryAsync(userId, categoryCode);
                    return Results.Ok(ApiResponse<ListExpensesResponse>.Success(
                        new ListExpensesResponse { Expenses = result.ToList() },
                        "Expenses retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetExpensesByCategory")
            .Produces<ApiResponse<ListExpensesResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Update expense
            app.MapPut("/{publicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Update expense",
                    Description = "Updates an existing expense."
                )]
            async (Guid publicId, [FromBody] UpdateExpenseRequest request, IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await expenseService.UpdateExpenseAsync(publicId, userId, request);
                    return Results.Ok(ApiResponse<ExpenseResponse>.Success(result, "Expense updated successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<UpdateExpenseRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("UpdateExpense")
            .Produces<ApiResponse<ExpenseResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Delete expense
            app.MapDelete("/{publicId:guid}",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Delete expense",
                    Description = "Deletes an expense."
                )]
            async (Guid publicId, IExpenseService expenseService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    await expenseService.DeleteExpenseAsync(publicId, userId);
                    return Results.Ok(ApiResponse<EmptyResponse>.Success(new EmptyResponse(), "Expense deleted successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("DeleteExpense")
            .Produces<ApiResponse<EmptyResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);
        }
    }
}
