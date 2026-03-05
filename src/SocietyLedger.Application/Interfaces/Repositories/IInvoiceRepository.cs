using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<IEnumerable<Invoice>> GetByUserIdAsync(long userId);
        Task CreateAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task<string> GenerateInvoiceNumberAsync();
    }
}