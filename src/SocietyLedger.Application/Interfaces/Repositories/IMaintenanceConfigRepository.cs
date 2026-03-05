using SocietyLedger.Application.DTOs.MaintenanceConfig;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IMaintenanceConfigRepository
    {
        /// <summary>Returns the config for a society, or null if it doesn't exist yet.</summary>
        Task<MaintenanceConfigResponse?> GetBySocietyIdAsync(long societyId);

        /// <summary>Creates or updates the maintenance config for a society (upsert).</summary>
        Task UpsertAsync(long societyId, Guid societyPublicId, SaveMaintenanceConfigRequest request, long changedByUserId);
    }
}
