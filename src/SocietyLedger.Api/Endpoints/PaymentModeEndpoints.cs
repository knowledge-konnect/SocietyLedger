using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using SocietyLedger.Application.DTOs.MaintenancePayment;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class PaymentModeEndpoints
    {
        public static void MapPaymentModeRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Get all payment modes
            app.MapGet("/",
                [Authorize]
            [SwaggerOperation(
                    Summary = "Get payment modes",
                    Description = "Retrieves all available payment modes for maintenance payments."
                )]
            async (IMaintenancePaymentService paymentService, HttpContext ctx) =>
                {
                    var result = await paymentService.GetPaymentModesAsync();
                    return Results.Ok(ApiResponse<ListPaymentModesResponse>.Success(
                        new ListPaymentModesResponse(result.ToList()),
                        "Payment modes retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetPaymentModes")
            .Produces<ApiResponse<ListPaymentModesResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);
        }
    }
}
