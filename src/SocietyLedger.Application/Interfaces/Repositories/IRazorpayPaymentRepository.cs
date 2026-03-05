using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByRazorpayOrderIdAsync(string orderId);
        Task<Payment?> GetByRazorpayPaymentIdAsync(string paymentId);
        Task<Payment?> GetPendingSubscriptionPaymentByUserIdAsync(long userId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task SaveChangesAsync();
    }
}