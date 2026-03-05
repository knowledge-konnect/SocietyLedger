using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.DTOs.MaintenanceConfig;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;
using System.Text.Json;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class MaintenanceConfigRepository : IMaintenanceConfigRepository
    {
        private readonly AppDbContext _db;

        public MaintenanceConfigRepository(AppDbContext db) => _db = db;

        public async Task<MaintenanceConfigResponse?> GetBySocietyIdAsync(long societyId)
        {
            var config = await _db.maintenance_configs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.society_id == societyId);

            if (config == null)
                return null;

            return MapToResponse(config);
        }

        public async Task UpsertAsync(long societyId, Guid societyPublicId, SaveMaintenanceConfigRequest request, long changedByUserId)
        {
            var existing = await _db.maintenance_configs
                .FirstOrDefaultAsync(c => c.society_id == societyId);

            if (existing == null)
            {
                existing = new maintenance_config
                {
                    public_id = Guid.NewGuid(),
                    society_id = societyId,
                    default_monthly_charge = request.DefaultMonthlyCharge,
                    due_day_of_month = request.DueDayOfMonth,
                    late_fee_per_month = request.LateFeePerMonth,
                    grace_period_days = request.GracePeriodDays,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow,
                    created_by = changedByUserId,
                    updated_by = changedByUserId
                };
                _db.maintenance_configs.Add(existing);

                // Save first so that the generated id is available for audit log
                await _db.SaveChangesAsync();

                _db.audit_logs.Add(new audit_log
                {
                    society_id = societyId,
                    table_name = "maintenance_config",
                    record_id = existing.id,
                    record_public_id = existing.public_id,
                    action = "CREATE",
                    changed_by = changedByUserId,
                    changed_at = DateTime.UtcNow,
                    diff = null,
                    metadata = JsonSerializer.Serialize(new
                    {
                        request.DefaultMonthlyCharge,
                        request.DueDayOfMonth,
                        request.LateFeePerMonth,
                        request.GracePeriodDays
                    })
                });
                await _db.SaveChangesAsync();
            }
            else
            {
                var oldSnapshot = JsonSerializer.Serialize(MapToResponse(existing));

                existing.default_monthly_charge = request.DefaultMonthlyCharge;
                existing.due_day_of_month = request.DueDayOfMonth;
                existing.late_fee_per_month = request.LateFeePerMonth;
                existing.grace_period_days = request.GracePeriodDays;
                existing.updated_at = DateTime.UtcNow;
                existing.updated_by = changedByUserId;

                var newSnapshot = JsonSerializer.Serialize(new
                {
                    request.DefaultMonthlyCharge,
                    request.DueDayOfMonth,
                    request.LateFeePerMonth,
                    request.GracePeriodDays
                });

                _db.audit_logs.Add(new audit_log
                {
                    society_id = societyId,
                    table_name = "maintenance_config",
                    record_id = existing.id,
                    record_public_id = existing.public_id,
                    action = "UPDATE",
                    changed_by = changedByUserId,
                    changed_at = DateTime.UtcNow,
                    diff = JsonSerializer.Serialize(new { before = oldSnapshot, after = newSnapshot }),
                    metadata = newSnapshot
                });

                await _db.SaveChangesAsync();
            }
        }

        private static MaintenanceConfigResponse MapToResponse(maintenance_config config)
        {
            // Fetch society public_id via navigation? We only stored society_id.
            // Return a partial response; the service will populate SocietyPublicId.
            return new MaintenanceConfigResponse
            {
                DefaultMonthlyCharge = config.default_monthly_charge,
                DueDayOfMonth = config.due_day_of_month,
                LateFeePerMonth = config.late_fee_per_month,
                GracePeriodDays = config.grace_period_days
            };
        }
    }
}
