using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;

        public PaymentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Payment?> GetPendingSubscriptionPaymentByUserIdAsync(long userId)
        {
            var efPayment = await _db.payments
                .AsNoTracking()
                .Where(p => p.payment_type == PaymentTypeCodes.Subscription && p.razorpay_payment_id == null && p.society_id == userId && !p.is_deleted)
                .OrderByDescending(p => p.created_at)
                .FirstOrDefaultAsync();
            return efPayment?.ToDomain();
        }
        public async Task<Payment?> GetByRazorpayOrderIdAsync(string orderId)
        {
            var efPayment = await _db.payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.razorpay_order_id == orderId && !p.is_deleted);

            return efPayment?.ToDomain();
        }

        public async Task<Payment?> GetByRazorpayPaymentIdAsync(string paymentId)
        {
            var efPayment = await _db.payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.razorpay_payment_id == paymentId && !p.is_deleted);

            return efPayment?.ToDomain();
        }

        public async Task AddAsync(Payment payment)
        {
            var entity = payment.ToEntity();
            await _db.payments.AddAsync(entity);
        }

        public async Task UpdateAsync(Payment payment)
        {
            var entity = payment.ToEntity();
            _db.payments.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}