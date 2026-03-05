namespace SocietyLedger.Application.DTOs.Invoice
{
    public class ListInvoicesResponse
    {
        public List<InvoiceResponse> Invoices { get; set; } = new();
    }
}
