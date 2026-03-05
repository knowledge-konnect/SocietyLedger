using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _db;

        public InvoiceRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            var efInvoice = await _db.invoices
                .Include(i => i.subscription)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.id == id);

            return efInvoice?.ToDomain();
        }

        public async Task<IEnumerable<Invoice>> GetByUserIdAsync(long userId)
        {
            var efInvoices = await _db.invoices
                .Include(i => i.subscription)
                .Where(i => i.user_id == userId)
                .OrderByDescending(i => i.created_at)
                .AsNoTracking()
                .ToListAsync();

            return efInvoices.Select(i => i.ToDomain());
        }

        public async Task CreateAsync(Invoice invoice)
        {
            var efInvoice = invoice.ToEntity();
            efInvoice.created_at = DateTime.UtcNow;
            efInvoice.updated_at = DateTime.UtcNow;
            _db.invoices.Add(efInvoice);
            await _db.SaveChangesAsync();
            invoice.Id = efInvoice.id;
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            var efInvoice = invoice.ToEntity();
            efInvoice.updated_at = DateTime.UtcNow;
            _db.invoices.Update(efInvoice);
            await _db.SaveChangesAsync();
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var now = DateTime.UtcNow;
            var year = now.Year;
            var month = now.Month.ToString("D2");

            // Get the next invoice number for this month
            var lastInvoice = await _db.invoices
                .Where(i => i.invoice_number.StartsWith($"INV-{year}{month}"))
                .OrderByDescending(i => i.invoice_number)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null)
            {
                var parts = lastInvoice.invoice_number.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            return $"INV-{year}{month}-{nextNumber:D4}";
        }
    }
}