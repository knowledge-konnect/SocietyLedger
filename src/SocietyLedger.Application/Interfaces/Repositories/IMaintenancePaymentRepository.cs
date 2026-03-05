namespace SocietyLedger.Application.Interfaces.Repositories
{
    using SocietyLedger.Application.DTOs.MaintenancePayment;

    public interface IMaintenancePaymentRepository
    {
        Task<MaintenancePaymentEntity?> GetByPublicIdAsync(Guid publicId, long societyId);
        Task<IEnumerable<MaintenancePaymentEntity>> GetBySocietyIdAsync(long societyId);
        Task<IEnumerable<MaintenancePaymentEntity>> GetByFlatPublicIdAsync(Guid flatPublicId);
        Task<MaintenancePaymentEntity> CreateAsync(MaintenancePaymentEntity payment);
        Task UpdateByPublicIdAsync(Guid publicId, long societyId, Action<MaintenancePaymentEntity> updateAction);
        Task DeleteByPublicIdAsync(Guid publicId, long societyId);
    }
}
