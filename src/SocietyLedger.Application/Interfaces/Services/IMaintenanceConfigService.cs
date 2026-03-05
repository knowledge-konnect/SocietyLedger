using SocietyLedger.Application.DTOs.MaintenanceConfig;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IMaintenanceConfigService
    {
        /// <summary>Get maintenance configuration for a society. Returns defaults if none configured yet.</summary>
        Task<MaintenanceConfigResponse> GetAsync(Guid societyPublicId, long authUserId);

        /// <summary>Upsert maintenance configuration for a society. Logs to audit table.</summary>
        Task<MaintenanceConfigResponse> SaveAsync(Guid societyPublicId, SaveMaintenanceConfigRequest request, long authUserId);
    }
}
