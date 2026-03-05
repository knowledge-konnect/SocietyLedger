using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.Auth;
using SocietyLedger.Application.DTOs.User;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;
namespace SocietyLedger.Api.Endpoints
{
    public static class AuthRoutes
    {
        // ---------------------------------------------------------------------------
        // Cookie helpers
        // ---------------------------------------------------------------------------

        private const string RefreshTokenCookieName = "refreshToken";

        /// <summary>
        /// Cookie path scoped to auth routes only.
        /// Adjust if the API is mounted under a versioned prefix (e.g. "/v1/auth").
        /// </summary>
        private const string RefreshTokenCookiePath = "/auth";

        private static void SetRefreshTokenCookie(
            HttpContext ctx,
            string refreshToken,
            DateTime expiresAt,
            IWebHostEnvironment env)
        {
            ctx.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure   = !env.IsDevelopment(),   // true in staging/production, false in local dev
                SameSite = SameSiteMode.Strict,
                Path     = RefreshTokenCookiePath,
                Expires  = new DateTimeOffset(DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc))
            });
        }

        private static void ClearRefreshTokenCookie(HttpContext ctx, IWebHostEnvironment env)
        {
            ctx.Response.Cookies.Append(RefreshTokenCookieName, string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure   = !env.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Path     = RefreshTokenCookiePath,
                Expires  = DateTimeOffset.UnixEpoch,
                MaxAge   = TimeSpan.Zero
            });
        }

        // ---------------------------------------------------------------------------
        // Routes
        // ---------------------------------------------------------------------------

        public static void MapAuthRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Register
            app.MapPost("/register",
                [AllowAnonymous]
            [SwaggerOperation(
                    Summary = "Register user",
                    Description = "Creates a new user account in the system."
                )]
            async ([FromBody] RegisterRequest request, IAuthService authService, HttpContext ctx, IWebHostEnvironment env) =>
                {
                    var ip = ctx.GetClientIp();
                    var res = await authService.RegisterAsync(request, ip);
                    // Deliver refresh token as httpOnly cookie; [JsonIgnore] keeps it out of the body.
                    SetRefreshTokenCookie(ctx, res.RefreshToken, res.RefreshTokenExpiresAt, env);
                    Log.Information("User registration successful for {Email}", request.Email);
                    return Results.Ok(ApiResponse<RegisterResponse>.Success(res, "Account created successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<RegisterRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("Register")
            .Produces<ApiResponse<RegisterResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(409)
            .Produces<ErrorResponse>(500);


            // Login
            app.MapPost("/login",
                [AllowAnonymous]
            [SwaggerOperation(
                    Summary = "Login user",
                    Description = "Authenticates a user and returns an access token. The refresh token is set as an httpOnly cookie."
                )]
            async ([FromBody] LoginRequest request, IAuthService authService, HttpContext ctx, IWebHostEnvironment env) =>
                {
                    var ip = ctx.GetClientIp();
                    var res = await authService.LoginAsync(request, ip);
                    SetRefreshTokenCookie(ctx, res.RefreshToken, res.RefreshTokenExpiresAt, env);
                    Log.Information("Login successful for {User}", request.UsernameOrEmail);
                    return Results.Ok(ApiResponse<LoginResponse>.Success(res, "Logged in successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<LoginRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("Login")
            .Produces<ApiResponse<LoginResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);


            // Refresh Token
            app.MapPost("/refresh",
                [AllowAnonymous]
            [SwaggerOperation(
                    Summary = "Refresh token",
                    Description = "Rotates the refresh token (read from the httpOnly cookie) and returns a new access token."
                )]
            async (IAuthService authService, HttpContext ctx, IWebHostEnvironment env) =>
                {
                    var ip = ctx.GetClientIp();

                    // Refresh token must arrive via the httpOnly cookie — never the request body.
                    var refreshToken = ctx.Request.Cookies[RefreshTokenCookieName];
                    if (string.IsNullOrWhiteSpace(refreshToken))
                    {
                        var errorResponse = ErrorResponse.Create(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_TOKEN, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var res = await authService.RefreshTokenAsync(refreshToken, ip);
                    // Rotate: overwrite the cookie with the newly issued refresh token.
                    SetRefreshTokenCookie(ctx, res.RefreshToken, res.RefreshTokenExpiresAt, env);
                    Log.Information("Refresh token successful for IP {Ip}", ip);
                    return Results.Ok(ApiResponse<LoginResponse>.Success(res, "Token refreshed successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("Refresh")
            .Produces<ApiResponse<LoginResponse>>(200)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);


            // Revoke Token (logout)
            app.MapPost("/revoke",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Revoke refresh token",
                    Description = "Revokes the refresh token (read from the httpOnly cookie) and clears the cookie."
                )]
            async (IAuthService authService, HttpContext ctx, IWebHostEnvironment env) =>
                {
                    var ip = ctx.GetClientIp();

                    var refreshToken = ctx.Request.Cookies[RefreshTokenCookieName];
                    if (string.IsNullOrWhiteSpace(refreshToken))
                    {
                        var errorResponse = ErrorResponse.Create(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                    await authService.RevokeRefreshTokenAsync(refreshToken, ip);
                    ClearRefreshTokenCookie(ctx, env);
                    Log.Information("Refresh token revoked for IP {Ip}", ip);
                    return Results.Ok(ApiResponse<EmptyResponse>.Success(null, "Token revoked successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("Revoke")
            .Produces<ApiResponse<EmptyResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);

            // Change Password
            app.MapPost("/change-password",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Change user password",
                    Description = "Allows an authenticated user to change their password. Requires verification of current password."
                )]
            async ([FromBody] ChangePasswordRequest request, IAuthService authService, HttpContext ctx) =>
                {
                    var userId = ctx.GetAuthenticatedUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized change password attempt - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var res = await authService.ChangePasswordAsync(userId, request);
                    Log.Information("Password changed successfully for user {UserId}", userId);
                    return Results.Ok(ApiResponse<ChangePasswordResponse>.Success(res, "Password changed successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<ChangePasswordRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("ChangePassword")
            .Produces<ApiResponse<ChangePasswordResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(404)
            .Produces<ErrorResponse>(500);

            // Update own profile (self-service)
            app.MapPatch("/profile",
                [Authorize]
                [SwaggerOperation(
                    Summary = "Update own profile",
                    Description = "Allows an authenticated user to update their own mobile number. Email and role changes are not permitted."
                )]
                async ([FromBody] UpdateProfileRequest request, IUserService userService, HttpContext ctx) =>
                {
                    var userId = ctx.GetAuthenticatedUserId();
                    if (userId == 0)
                    {
                        Log.Warning("Unauthorized profile update attempt - invalid user ID");
                        var errorResponse = ErrorResponse.Create(ErrorCodes.UNAUTHORIZED, ErrorMessages.UNAUTHORIZED, ctx.TraceIdentifier);
                        return Results.Json(errorResponse, statusCode: 401);
                    }

                    var profile = await userService.UpdateProfileAsync(userId, request);
                    Log.Information("Profile updated for user {UserId}", userId);
                    return Results.Ok(ApiResponse<ProfileResponse>.Success(profile, "Profile updated successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<UpdateProfileRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("UpdateProfile")
            .Produces<ApiResponse<ProfileResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(401)
            .Produces<ErrorResponse>(500);

            app.MapGet("user",
    [SwaggerOperation(
        Summary = "Get current user",
        Description = "Returns the currently authenticated user's details."
    )]
            async (IUserService userService, HttpContext ctx) =>
    {
        var userId = ctx.GetAuthenticatedUserId();
        if (userId == 0)
        {
            var errorResponse = ErrorResponse.Create("UNAUTHORIZED", "User not authenticated", ctx.TraceIdentifier);
            return Results.Json(errorResponse, statusCode: 401);
        }

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            var errorResponse = ErrorResponse.Create("USER_NOT_FOUND", "User not found", ctx.TraceIdentifier);
            return Results.Json(errorResponse, statusCode: 404);
        }

        return Results.Ok(ApiResponse<UserResponseDto>.Success(user));
    })
    .RequireAuthorization()
    .WithTags(groupName)
    .WithApiVersionSet(versionSet)
    .HasApiVersion(version_1_0)
    .WithName("GetCurrentUser")
    .Produces<ApiResponse<UserResponseDto>>(200)
    .Produces<ErrorResponse>(401)
    .Produces<ErrorResponse>(404)
    .Produces<ErrorResponse>(500);
        }
    }
}
