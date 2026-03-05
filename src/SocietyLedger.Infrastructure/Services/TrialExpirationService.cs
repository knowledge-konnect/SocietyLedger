using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Application.Interfaces.Services;

namespace SocietyLedger.Infrastructure.Services
{
    public class TrialExpirationService : BackgroundService
    {
        private readonly ILogger<TrialExpirationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Check daily

        public TrialExpirationService(
            ILogger<TrialExpirationService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Trial expiration service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpiredTrialsAsync();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in trial expiration check");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Retry after 5 minutes on error
                }
            }
        }

        private async Task CheckExpiredTrialsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var subscriptionRepo = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
            var eventRepo = scope.ServiceProvider.GetRequiredService<ISubscriptionEventRepository>();

            var expiredTrials = await subscriptionRepo.GetExpiredTrialsAsync();
            var now = DateTime.UtcNow;

            foreach (var subscription in expiredTrials)
            {
                try
                {
                    subscription.Status = SubscriptionStatusCodes.Expired;
                    subscription.UpdatedAt = now;
                    await subscriptionRepo.UpdateAsync(subscription);

                    // Create subscription event
                    await eventRepo.CreateAsync(new SubscriptionEvent
                    {
                        Id = Guid.NewGuid(),
                        UserId = subscription.UserId,
                        SubscriptionId = subscription.Id,
                        EventType = "trial_expired",
                        OldStatus = SubscriptionStatusCodes.Trial,
                        NewStatus = SubscriptionStatusCodes.Expired,
                        Metadata = $"{{\"trial_end\":\"{subscription.TrialEnd}\"}}"
                    });

                    _logger.LogInformation("Trial expired for user {UserId}, subscription {SubscriptionId}",
                        subscription.UserId, subscription.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing expired trial for user {UserId}", subscription.UserId);
                }
            }

            if (expiredTrials.Any())
            {
                _logger.LogInformation("Processed {Count} expired trials", expiredTrials.Count());
            }
        }
    }
}