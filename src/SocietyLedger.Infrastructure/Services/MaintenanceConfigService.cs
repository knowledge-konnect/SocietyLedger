using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.MaintenanceConfig;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Exceptions;

namespace SocietyLedger.Infrastructure.Services
{
    public class MaintenanceConfigService : IMaintenanceConfigService
    {
        private readonly IMaintenanceConfigRepository _configRepo;
        private readonly ISocietyRepository _societyRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<MaintenanceConfigService> _logger;

        public MaintenanceConfigService(
            IMaintenanceConfigRepository configRepo,
            ISocietyRepository societyRepo,
            IUserRepository userRepo,
            ILogger<MaintenanceConfigService> logger)
        {
            _configRepo = configRepo;
            _societyRepo = societyRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<MaintenanceConfigResponse> GetAsync(Guid societyPublicId, long authUserId)
        {
            await ValidateAccessAsync(societyPublicId, authUserId, "read maintenance config");

            var society = await _societyRepo.GetByPublicIdAsync(societyPublicId)
                ?? throw new NotFoundException("Society", societyPublicId.ToString());

            var config = await _configRepo.GetBySocietyIdAsync(society.Id);

            // Return defaults if no config exists yet
            if (config == null)
            {
                return new MaintenanceConfigResponse
                {
                    SocietyPublicId = societyPublicId,
                    DefaultMonthlyCharge = 0,
                    DueDayOfMonth = 1,
                    LateFeePerMonth = 0,
                    GracePeriodDays = 0
                };
            }

            config.SocietyPublicId = societyPublicId;
            return config;
        }

        public async Task<MaintenanceConfigResponse> SaveAsync(Guid societyPublicId, SaveMaintenanceConfigRequest request, long authUserId)
        {
            await ValidateAccessAsync(societyPublicId, authUserId, "save maintenance config");

            var society = await _societyRepo.GetByPublicIdAsync(societyPublicId)
                ?? throw new NotFoundException("Society", societyPublicId.ToString());

            await _configRepo.UpsertAsync(society.Id, societyPublicId, request, authUserId);

            _logger.LogInformation("Maintenance config saved for society {SocietyPublicId} by user {UserId}", societyPublicId, authUserId);

            return new MaintenanceConfigResponse
            {
                SocietyPublicId = societyPublicId,
                DefaultMonthlyCharge = request.DefaultMonthlyCharge,
                DueDayOfMonth = request.DueDayOfMonth,
                LateFeePerMonth = request.LateFeePerMonth,
                GracePeriodDays = request.GracePeriodDays
            };
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        private async Task ValidateAccessAsync(Guid societyPublicId, long authUserId, string operation)
        {
            var authUser = await _userRepo.GetByIdAsync(authUserId);
            if (authUser == null || !authUser.IsActive)
                throw new AuthenticationException("Invalid or missing authentication token.");

            // Verify the user belongs to the requested society
            if (authUser.SocietyPublicId != societyPublicId)
                throw new AuthorizationException("You can only access maintenance configuration for your own society.");

            // Verify the user has Admin or Treasurer role
            var role = authUser.Role?.Code ?? string.Empty;
            if (role != RoleCodes.Admin && role != RoleCodes.Treasurer && role != RoleCodes.SocietyAdmin)
                throw new AuthorizationException("Only Admin or Treasurer users can access maintenance configuration.");
        }
    }
}
