using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.Razorpay;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;
using System.Text.Json;

namespace SocietyLedger.Api.Endpoints
{
    public static class PaymentEndpoints
    {
        public static void MapPaymentRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Create Razorpay order
            app.MapPost("/create-order",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Create Razorpay payment order",
                    Description = "Creates a Razorpay order for subscription payment."
                )]
            async ([FromBody] CreateOrderRequest request, IRazorpayPaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await paymentService.CreateOrderAsync(userId, request.PlanId);
                    return Results.Ok(ApiResponse<CreateOrderResponse>.Success(result, "Order created successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<CreateOrderRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("CreateOrder")
            .Produces<ApiResponse<CreateOrderResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Verify payment
            app.MapPost("/verify-payment",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Verify Razorpay payment",
                    Description = "Verifies payment signature and activates subscription."
                )]
            async ([FromBody] VerifyPaymentRequest request, IRazorpayPaymentService paymentService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await paymentService.VerifyPaymentAsync(request);
                    return Results.Ok(ApiResponse<VerifyPaymentResponse>.Success(result, "Payment verification completed"));
                })
            .AddEndpointFilter<FluentValidationFilter<VerifyPaymentRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("VerifyPayment")
            .Produces<ApiResponse<VerifyPaymentResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Webhook endpoint (no auth — protected by HMAC-SHA256 signature verification inside the service)
            app.MapPost("/webhook",
                [AllowAnonymous]
            [SwaggerOperation(
                    Summary = "Razorpay webhook",
                    Description = "Handles Razorpay payment events. Signature is verified server-side using X-Razorpay-Signature header."
                )]
            async (HttpRequest req, IRazorpayPaymentService paymentService) =>
                {
                    req.EnableBuffering();
                    using var reader = new StreamReader(req.Body, Encoding.UTF8, leaveOpen: true);
                    var rawBody = await reader.ReadToEndAsync();
                    req.Body.Position = 0;

                    var signature = req.Headers["X-Razorpay-Signature"].ToString();

                    WebhookPayload? payload;
                    try
                    {
                        payload = JsonSerializer.Deserialize<WebhookPayload>(
                            rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (JsonException)
                    {
                        Log.Warning("Webhook: failed to deserialize payload");
                        return Results.BadRequest(ErrorResponse.Create(ErrorCodes.VALIDATION_FAILED, "Invalid webhook payload", null));
                    }

                    if (payload == null)
                        return Results.BadRequest(ErrorResponse.Create(ErrorCodes.VALIDATION_ERROR, "Empty webhook payload", null));

                    await paymentService.ProcessWebhookAsync(rawBody, signature, payload);
                    return Results.Ok(ApiResponse<string>.Success("ok", "Webhook processed"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("PaymentWebhook")
            .Produces<ApiResponse<string>>(200);
        }
    }
}