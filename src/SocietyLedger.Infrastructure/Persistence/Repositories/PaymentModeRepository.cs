using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.DTOs.MaintenancePayment;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class PaymentModeRepository : IPaymentModeRepository
    {
        private readonly AppDbContext _db;

        public PaymentModeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PaymentModeEntity?> GetByIdAsync(short id)
        {
            var mode = await _db.payment_modes
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.id == id);

            return mode != null ? MapToDto(mode) : null;
        }

        public async Task<PaymentModeEntity?> GetByCodeAsync(string code)
        {
            var mode = await _db.payment_modes
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.code == code);

            return mode != null ? MapToDto(mode) : null;
        }

        public async Task<IEnumerable<PaymentModeEntity>> GetAllAsync()
        {
            var modes = await _db.payment_modes
                .OrderBy(pm => pm.display_name)
                .AsNoTracking()
                .ToListAsync();

            return modes.Select(MapToDto);
        }

        private static PaymentModeEntity MapToDto(payment_mode mode)
        {
            return new PaymentModeEntity(
                mode.id,
                mode.code,
                mode.display_name
            );
        }
    }
}
