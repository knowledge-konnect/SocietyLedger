using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.User;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class UserRoutes
    {
        public static void MapUserRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // GET /users - List users for authenticated user's society
            app.MapGet("/",
                [Authorize("ActiveSubscription")]
                [SwaggerOperation(
                    Summary = "Get Users",
                    Description = "Fetches list of users in the authenticated user's society. Admin only.")]
                async (
                    HttpContext ctx,
                    IUserService userService) =>
                {
                    var authUserId = ctx.GetAuthenticatedUserId();
                    if (authUserId == 0)
                    {
                        Log.Warning("Unauthorized user list request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var users = await userService.GetUsersForAdminAsync(authUserId);
                    Log.Information("Users listed for society by user {UserId}", authUserId);
                    return Results.Ok(ApiResponse<ListUsersResponse>.Success(
                        new ListUsersResponse { Users = users.ToList() },
                        "Users retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("ListUsers")
            .Produces<ApiResponse<ListUsersResponse>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(500);

            // GET /users/{publicId} - Get user by ID
            app.MapGet("/{publicId:guid}",
                [Authorize("ActiveSubscription")]
                [SwaggerOperation(
                    Summary = "Get User",
                    Description = "Fetches user details by public ID. Admin only. Society isolation enforced.")]
                async (
                    Guid publicId,
                    HttpContext ctx,
                    IUserService userService) =>
                {
                    var authUserId = ctx.GetAuthenticatedUserId();
                    if (authUserId == 0)
                    {
                        Log.Warning("Unauthorized user get request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var user = await userService.GetUserByPublicIdForAdminAsync(publicId, authUserId);
                    Log.Information("User {PublicId} fetched by {UserId}", publicId, authUserId);
                    return Results.Ok(ApiResponse<UserResponseDto>.Success(user, "User retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetUser")
            .Produces<ApiResponse<UserResponseDto>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);

            // POST /users - Create user
            app.MapPost("/",
                [Authorize("ActiveSubscription")]
                [SwaggerOperation(
                    Summary = "Create User",
                    Description = "Creates a new user in the authenticated user's society. Admin only. Admin must provide the initial password.")]
                async (
                    [FromBody] CreateUserDto request,
                    HttpContext ctx,
                    IUserService userService) =>
                {
                    var authUserId = ctx.GetAuthenticatedUserId();
                    if (authUserId == 0)
                    {
                        Log.Warning("Unauthorized user create request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var created = await userService.CreateUserForAdminAsync(request, authUserId);
                    Log.Information("User {Email} created by {UserId}", request.Email, authUserId);
                    return Results.Created(string.Empty, ApiResponse<CreateUserResponseDto>.Success(created, "User created successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<CreateUserDto>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("CreateUser")
            .Produces<ApiResponse<CreateUserResponseDto>>(201)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(409)
            .Produces<ErrorResponse>(500);

            // PUT /users/{publicId} - Update user
            app.MapPut("/{publicId:guid}",
                [Authorize("ActiveSubscription")]
                [SwaggerOperation(
                    Summary = "Update User",
                    Description = "Updates user details. Admin only. Society isolation enforced.")]
                async (
                    Guid publicId,
                    [FromBody] UpdateUserDto request,
                    HttpContext ctx,
                    IUserService userService) =>
                {
                    var authUserId = ctx.GetAuthenticatedUserId();
                    if (authUserId == 0)
                    {
                        Log.Warning("Unauthorized user update request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    if (publicId != request.PublicId)
                    {
                        Log.Warning("PublicId mismatch: URL {UrlId} != Body {BodyId}", publicId, request.PublicId);
                        var errorResponse = ErrorResponse.Create(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                    var updated = await userService.UpdateUserForAdminAsync(request, authUserId);
                    Log.Information("User {PublicId} updated by {UserId}", publicId, authUserId);
                    return Results.Ok(ApiResponse<UserResponseDto>.Success(updated, "User updated successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<UpdateUserDto>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("UpdateUser")
            .Produces<ApiResponse<UserResponseDto>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(409)
            .Produces<ErrorResponse>(500);

            // DELETE /users/{publicId} - Soft delete user
            app.MapDelete("/{publicId:guid}",
                [Authorize("ActiveSubscription")]
                [SwaggerOperation(
                    Summary = "Delete User",
                    Description = "Soft deletes a user (sets is_active = false). Admin only. Society isolation enforced.")]
                async (
                    Guid publicId,
                    HttpContext ctx,
                    IUserService userService) =>
                {
                    var authUserId = ctx.GetAuthenticatedUserId();
                    if (authUserId == 0)
                    {
                        Log.Warning("Unauthorized user delete request - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var deleted = await userService.DeleteUserForAdminAsync(publicId, authUserId);
                    Log.Information("User {PublicId} soft deleted by {UserId}", publicId, authUserId);
                    return Results.Ok(ApiResponse<EmptyResponse>.Success(null, "User deleted successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("DeleteUser")
            .Produces<ApiResponse<EmptyResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(403)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);
        }
    }
}
