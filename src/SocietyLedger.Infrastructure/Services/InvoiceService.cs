using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.Invoice;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly ISubscriptionEventRepository _eventRepo;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            ISubscriptionEventRepository eventRepo,
            ILogger<InvoiceService> logger)
        {
            _invoiceRepo = invoiceRepo;
            _eventRepo = eventRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<InvoiceResponse>> GetUserInvoicesAsync(long userId)
        {
            var invoices = await _invoiceRepo.GetByUserIdAsync(userId);
            return invoices.Select(i => new InvoiceResponse
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceType = i.InvoiceType,
                Amount = i.Amount,
                TaxAmount = i.TaxAmount,
                TotalAmount = i.TotalAmount,
                Currency = i.Currency,
                Status = i.Status,
                PeriodStart = i.PeriodStart,
                PeriodEnd = i.PeriodEnd,
                DueDate = i.DueDate,
                PaidDate = i.PaidDate,
                PaymentMethod = i.PaymentMethod,
                PaymentReference = i.PaymentReference,
                Description = i.Description,
                CreatedAt = i.CreatedAt
            });
        }

        public async Task<InvoiceResponse> PayInvoiceAsync(Guid invoiceId, PayInvoiceRequest request)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new NotFoundException("Invoice", invoiceId.ToString());

            if (invoice.Status == InvoiceStatusCodes.Paid)
                throw new ConflictException("Invoice is already paid");

            var now = DateTime.UtcNow;
            var amount = request.Amount ?? invoice.TotalAmount;

            invoice.Status = InvoiceStatusCodes.Paid;
            invoice.PaidDate = now;
            invoice.PaymentMethod = request.PaymentMethod;
            invoice.PaymentReference = request.PaymentReference;
            invoice.UpdatedAt = now;

            await _invoiceRepo.UpdateAsync(invoice);

            // Create subscription event if this is a subscription invoice
            if (invoice.SubscriptionId.HasValue)
            {
                await _eventRepo.CreateAsync(new SubscriptionEvent
                {
                    Id = Guid.NewGuid(),
                    UserId = invoice.UserId,
                    SubscriptionId = invoice.SubscriptionId,
                    EventType = "payment_received",
                    Amount = amount,
                    Metadata = $"{{\"invoice_id\":\"{invoiceId}\",\"payment_method\":\"{request.PaymentMethod}\"}}"
                });
            }

            _logger.LogInformation("Invoice {InvoiceId} paid by user {UserId}", invoiceId, invoice.UserId);

            return new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceType = invoice.InvoiceType,
                Amount = invoice.Amount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                Currency = invoice.Currency,
                Status = invoice.Status,
                PeriodStart = invoice.PeriodStart,
                PeriodEnd = invoice.PeriodEnd,
                DueDate = invoice.DueDate,
                PaidDate = invoice.PaidDate,
                PaymentMethod = invoice.PaymentMethod,
                PaymentReference = invoice.PaymentReference,
                Description = invoice.Description,
                CreatedAt = invoice.CreatedAt
            };
        }
        }
   
}