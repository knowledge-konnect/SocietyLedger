using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.DTOs.MaintenancePayment;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;
using SocietyLedger.Infrastructure.Persistence.Extensions;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class MaintenancePaymentRepository : IMaintenancePaymentRepository
    {
        private readonly AppDbContext _db;

        public MaintenancePaymentRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<MaintenancePaymentEntity?> GetByPublicIdAsync(Guid publicId, long societyId)
        {
            var payment = await _db.maintenance_payments
                .ForSociety(societyId)
                .Include(mp => mp.flat)
                .Include(mp => mp.payment_mode)
                .Include(mp => mp.recorded_byNavigation)
                .Include(mp => mp.society)
                .Include(mp => mp.bill)
                .AsNoTracking()
                .FirstOrDefaultAsync(mp => mp.public_id == publicId);

            return payment != null ? MapToDto(payment) : null;
        }

        public async Task<IEnumerable<MaintenancePaymentEntity>> GetBySocietyIdAsync(long societyId)
        {
            var payments = await _db.maintenance_payments
                .ForSociety(societyId)
                .Include(mp => mp.flat)
                .Include(mp => mp.payment_mode)
                .Include(mp => mp.recorded_byNavigation)
                .Include(mp => mp.society)
                .Include(mp => mp.bill)
                .OrderByDescending(mp => mp.payment_date)
                .AsNoTracking()
                .ToListAsync();

            return payments.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenancePaymentEntity>> GetByFlatPublicIdAsync(Guid flatPublicId)
        {
            var payments = await _db.maintenance_payments
                .Include(mp => mp.flat)
                .Include(mp => mp.payment_mode)
                .Include(mp => mp.recorded_byNavigation)
                .Include(mp => mp.society)
                .Include(mp => mp.bill)
                .Where(mp => mp.flat!.public_id == flatPublicId && !mp.is_deleted)
                .OrderByDescending(mp => mp.payment_date)
                .AsNoTracking()
                .ToListAsync();

            return payments.Select(MapToDto);
        }

        public async Task<MaintenancePaymentEntity> CreateAsync(MaintenancePaymentEntity payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            // Get flat internal id from public_id
            var flat = await _db.flats
                .ForSociety(payment.SocietyId)
                .FirstOrDefaultAsync(f => f.public_id == payment.FlatPublicId);
            
            if (flat == null)
                throw new InvalidOperationException($"Flat with PublicId {payment.FlatPublicId} not found in society {payment.SocietyId}");

            var entity = new maintenance_payment
            {
                society_id = payment.SocietyId,
                flat_id = flat.id,
                amount = payment.Amount,
                payment_date = payment.PaymentDate,
                payment_mode_id = payment.PaymentModeId,
                reference_number = payment.ReferenceNumber,
                receipt_url = payment.ReceiptUrl,
                notes = payment.Notes,
                recorded_by = payment.RecordedBy,
                public_id = Guid.NewGuid(),
                created_at = DateTime.UtcNow
            };

            _db.maintenance_payments.Add(entity);
            await _db.SaveChangesAsync();

            return new MaintenancePaymentEntity
            {
                PublicId = entity.public_id,
                SocietyId = entity.society_id,
                SocietyPublicId = payment.SocietyPublicId,
                FlatPublicId = flat.public_id,
                FlatNumber = payment.FlatNumber,
                Amount = entity.amount,
                PaymentDate = entity.payment_date,
                PaymentModeId = entity.payment_mode_id,
                PaymentModeName = payment.PaymentModeName,
                ReferenceNumber = entity.reference_number,
                ReceiptUrl = entity.receipt_url,
                Notes = entity.notes,
                RecordedBy = entity.recorded_by,
                RecordedByName = payment.RecordedByName,
                CreatedAt = entity.created_at
            };
        }

        public async Task UpdateByPublicIdAsync(Guid publicId, long societyId, Action<MaintenancePaymentEntity> updateAction)
        {
            var payment = await _db.maintenance_payments
                .ForSociety(societyId)
                .FirstOrDefaultAsync(mp => mp.public_id == publicId);
            
            if (payment == null)
                throw new InvalidOperationException($"Maintenance payment with PublicId {publicId} not found in society {societyId}");

            // Load current state as DTO
            var paymentDto = new MaintenancePaymentEntity
            {
                PublicId = payment.public_id,
                SocietyId = payment.society_id,
                FlatPublicId = Guid.Empty, // Will be loaded if needed
                Amount = payment.amount,
                PaymentDate = payment.payment_date,
                PaymentModeId = payment.payment_mode_id,
                ReferenceNumber = payment.reference_number,
                ReceiptUrl = payment.receipt_url,
                Notes = payment.notes
            };

            // Apply updates
            updateAction(paymentDto);

            // Update entity
            payment.amount = paymentDto.Amount;
            payment.payment_date = paymentDto.PaymentDate;
            payment.payment_mode_id = paymentDto.PaymentModeId;
            payment.reference_number = paymentDto.ReferenceNumber;
            payment.receipt_url = paymentDto.ReceiptUrl;
            payment.notes = paymentDto.Notes;

            _db.maintenance_payments.Update(payment);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteByPublicIdAsync(Guid publicId, long societyId)
        {
            var payment = await _db.maintenance_payments
                .ForSociety(societyId)
                .FirstOrDefaultAsync(mp => mp.public_id == publicId);
            
            if (payment != null)
            {
                payment.is_deleted = true;
                payment.deleted_at = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        private MaintenancePaymentEntity MapToDto(maintenance_payment payment)
        {
            return new MaintenancePaymentEntity
            {
                PublicId        = payment.public_id,
                SocietyId       = payment.society_id,
                SocietyPublicId = payment.society?.public_id ?? Guid.Empty,
                FlatPublicId    = payment.flat?.public_id ?? Guid.Empty,
                FlatNumber      = payment.flat?.flat_no,
                Amount          = payment.amount,
                PaymentDate     = payment.payment_date,
                PaymentModeId   = payment.payment_mode_id,
                PaymentModeName = payment.payment_mode?.display_name,
                ReferenceNumber = payment.reference_number,
                ReceiptUrl      = payment.receipt_url,
                Notes           = payment.notes,
                RecordedBy      = payment.recorded_by,
                RecordedByName  = payment.recorded_byNavigation?.name,
                CreatedAt       = payment.created_at,
                BillPublicId    = payment.bill?.public_id,
                Period          = payment.bill?.period
            };
        }
    }
}
