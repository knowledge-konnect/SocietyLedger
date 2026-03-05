using Microsoft.Extensions.Logging;
using SocietyLedger.Application.DTOs.Subscription;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IPlanRepository _planRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly ISubscriptionEventRepository _eventRepo;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepo,
            IPlanRepository planRepo,
            IInvoiceRepository invoiceRepo,
            ISubscriptionEventRepository eventRepo,
            ILogger<SubscriptionService> logger)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _invoiceRepo = invoiceRepo;
            _eventRepo = eventRepo;
            _logger = logger;
        }

        public async Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(long userId)
        {
            var subscription = await _subscriptionRepo.GetByUserIdAsync(userId);

            if (subscription == null)
            {
                return new SubscriptionStatusResponse
                {
                    Status = "none",
                    AccessAllowed = false
                };
            }

            var now = DateTime.UtcNow;
            var accessAllowed = false;
            int? trialDaysRemaining = null;

            if (subscription.Status == SubscriptionStatusCodes.Trial)
            {
                if (subscription.TrialEnd > now)
                {
                    accessAllowed = true;
                    trialDaysRemaining = (subscription.TrialEnd - now)?.Days;
                }
                else
                {
                    accessAllowed = false; // Trial expired
                }
            }
            else if (subscription.Status == SubscriptionStatusCodes.Active)
            {
                accessAllowed = true;
            }
            else if (subscription.Status == SubscriptionStatusCodes.Cancelled)
            {
                // Allow access until period end if cancelled
                accessAllowed = subscription.CurrentPeriodEnd > now;
            }

            return new SubscriptionStatusResponse
            {
                Status = subscription.Status,
                TrialDaysRemaining = trialDaysRemaining,
                TrialEndDate = subscription.TrialEnd,
                AccessAllowed = accessAllowed,
                PlanName = subscription.Plan?.Name,
                MonthlyAmount = subscription.Plan?.MonthlyAmount,
                Currency = subscription.Plan?.Currency ?? "INR"
            };
        }

        public async Task<SubscribeResponse> SubscribeAsync(long userId, SubscribeRequest request)
        {
            var plan = await _planRepo.GetByIdAsync(request.PlanId);
            if (plan == null)
                throw new NotFoundException("Plan", request.PlanId.ToString());

            var existingSubscription = await _subscriptionRepo.GetByUserIdAsync(userId);
            if (existingSubscription != null && existingSubscription.Status == SubscriptionStatusCodes.Active)
                throw new ConflictException("User already has an active subscription");

            var amount = request.Amount ?? plan.MonthlyAmount;
            var now = DateTime.UtcNow;

            // Create or update subscription
            Subscription subscription;
            if (existingSubscription != null)
            {
                subscription = existingSubscription;
                subscription.Status = SubscriptionStatusCodes.Active;
                subscription.PlanId = request.PlanId;
                subscription.SubscribedAmount = amount;
                subscription.Currency = plan.Currency;
                subscription.CurrentPeriodStart = now;
                subscription.CurrentPeriodEnd = now.AddMonths(1);
                subscription.UpdatedAt = now;
                await _subscriptionRepo.UpdateAsync(subscription);
            }
            else
            {
                subscription = new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PlanId = request.PlanId,
                    Status = SubscriptionStatusCodes.Active,
                    SubscribedAmount = amount,
                    Currency = plan.Currency,
                    CurrentPeriodStart = now,
                    CurrentPeriodEnd = now.AddMonths(1),
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _subscriptionRepo.CreateAsync(subscription);
            }

            // Create invoice
            var invoiceNumber = await _invoiceRepo.GenerateInvoiceNumberAsync();
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscription.Id,
                InvoiceNumber = invoiceNumber,
                InvoiceType = PaymentTypeCodes.Subscription,
                Amount = amount,
                TotalAmount = amount,
                Currency = plan.Currency,
                Status = request.PaymentMethod.ToLower() == PaymentModeCodes.Razorpay ? InvoiceStatusCodes.Pending : InvoiceStatusCodes.Paid,
                DueDate = DateOnly.FromDateTime(now.AddDays(30)),
                PaymentMethod = request.PaymentMethod,
                PaymentReference = request.PaymentReference,
                Description = $"Subscription to {plan.Name} plan"
            };
            await _invoiceRepo.CreateAsync(invoice);

            // Create subscription event
            await _eventRepo.CreateAsync(new SubscriptionEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscription.Id,
                EventType = "subscribed",
                NewStatus = SubscriptionStatusCodes.Active,
                Amount = amount,
                Metadata = $"{{\"plan_id\":\"{request.PlanId}\",\"payment_method\":\"{request.PaymentMethod}\"}}"
            });

            _logger.LogInformation("User {UserId} subscribed to plan {PlanId} with payment method {PaymentMethod}", userId, request.PlanId, request.PaymentMethod);

            return new SubscribeResponse
            {
                SubscriptionId = subscription.Id,
                InvoiceId = invoice.Id,
                Status = invoice.Status,
                Amount = amount,
                InvoiceNumber = invoiceNumber,
                PaymentUrl = request.PaymentMethod.ToLower() == PaymentModeCodes.Razorpay ? "https://api.razorpay.com/v1/payment_links" : null // Placeholder
            };
        }

        public async Task CancelSubscriptionAsync(long userId, CancelSubscriptionRequest request)
        {
            var subscription = await _subscriptionRepo.GetByUserIdAsync(userId);
            if (subscription == null)
                throw new NotFoundException("Subscription", userId.ToString());

            if (subscription.Status != SubscriptionStatusCodes.Active)
                throw new ConflictException("Subscription is not active");

            var now = DateTime.UtcNow;
            subscription.Status = SubscriptionStatusCodes.Cancelled;
            subscription.CancelledAt = now;
            subscription.UpdatedAt = now;

            await _subscriptionRepo.UpdateAsync(subscription);

            // Create subscription event
            await _eventRepo.CreateAsync(new SubscriptionEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscription.Id,
                EventType = "cancelled",
                OldStatus = SubscriptionStatusCodes.Active,
                NewStatus = SubscriptionStatusCodes.Cancelled,
                Metadata = $"{{\"reason\":\"{request.Reason}\",\"cancel_immediately\":{request.CancelImmediately.ToString().ToLower()}}}"
            });

            _logger.LogInformation("User {UserId} cancelled subscription", userId);
        }

        public async Task CreateTrialSubscriptionAsync(long userId)
        {
            var existingSubscription = await _subscriptionRepo.GetByUserIdAsync(userId);
            if (existingSubscription != null)
                return; // Already has a subscription

            // Get the default plan (assuming there's a basic plan)
            var plans = await _planRepo.GetActivePlansAsync();
            var defaultPlan = plans.FirstOrDefault(p => p.Name.Contains("Basic") || p.Name.Contains("Free")) ?? plans.FirstOrDefault();

            if (defaultPlan == null)
                throw new NotFoundException("Plan", "default trial plan");

            var now = DateTime.UtcNow;
            var trialEnd = now.AddDays(30);

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PlanId = defaultPlan.Id,
                Status = SubscriptionStatusCodes.Trial,
                SubscribedAmount = 0,
                Currency = defaultPlan.Currency,
                TrialStart = now,
                TrialEnd = trialEnd,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _subscriptionRepo.CreateAsync(subscription);

            // Create subscription event
            await _eventRepo.CreateAsync(new SubscriptionEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscription.Id,
                EventType = "trial_started",
                NewStatus = SubscriptionStatusCodes.Trial,
                Metadata = $"{{\"trial_days\":30}}"
            });

            _logger.LogInformation("Created trial subscription for user {UserId}", userId);
        }
    }
}