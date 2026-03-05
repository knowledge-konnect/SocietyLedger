namespace SocietyLedger.Application.Interfaces.Repositories
{
    using SocietyLedger.Application.DTOs.MaintenancePayment;

    public interface IPaymentModeRepository
    {
        Task<PaymentModeEntity?> GetByIdAsync(short id);
        Task<PaymentModeEntity?> GetByCodeAsync(string code);
        Task<IEnumerable<PaymentModeEntity>> GetAllAsync();
    }
}
