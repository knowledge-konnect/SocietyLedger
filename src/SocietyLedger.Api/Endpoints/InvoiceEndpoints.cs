using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.DTOs.Invoice;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace SocietyLedger.Api.Endpoints
{
    public static class InvoiceEndpoints
    {
        public static void MapInvoiceRoutes(this RouteGroupBuilder app, string groupName, ApiVersionSet versionSet)
        {
            var version_1_0 = new ApiVersion(ApiConstants.API_VERSION_1_0);

            // Get user invoices
            app.MapGet("/",
                [Authorize("ActiveSubscription")]
            [SwaggerOperation(
                    Summary = "Get user invoices",
                    Description = "Returns all invoices for the authenticated user."
                )]
            async (IInvoiceService invoiceService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await invoiceService.GetUserInvoicesAsync(userId);
                    return Results.Ok(ApiResponse<ListInvoicesResponse>.Success(
                        new ListInvoicesResponse { Invoices = result.ToList() },
                        "Invoices retrieved successfully"));
                })
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("GetUserInvoices")
            .Produces<ApiResponse<ListInvoicesResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);

            // Pay invoice
            app.MapPost("/{invoiceId}/pay",
                [Authorize("ActiveSubscription")]
            [SwaggerOperation(
                    Summary = "Pay invoice",
                    Description = "Marks an invoice as paid with payment details."
                )]
            async (Guid invoiceId, [FromBody] PayInvoiceRequest request, IInvoiceService invoiceService, HttpContext ctx) =>
                {
                    var userId = ctx.GetUserId();
                    var result = await invoiceService.PayInvoiceAsync(invoiceId, request);
                    return Results.Ok(ApiResponse<InvoiceResponse>.Success(result, "Invoice paid successfully"));
                })
            .AddEndpointFilter<FluentValidationFilter<PayInvoiceRequest>>()
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version_1_0)
            .WithName("PayInvoice")
            .Produces<ApiResponse<InvoiceResponse>>(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(500);
        }
    }
}
