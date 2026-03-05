using SocietyLedger.Application.DTOs.Invoice;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponse>> GetUserInvoicesAsync(long userId);
        Task<InvoiceResponse> PayInvoiceAsync(Guid invoiceId, PayInvoiceRequest request);
    }
}