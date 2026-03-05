using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Razorpay.Api;
using SocietyLedger.Application.DTOs.Razorpay;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Domain.Constants;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace SocietyLedger.Infrastructure.Services
{
    public class RazorpayPaymentService : IRazorpayPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPlanService _planService;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<RazorpayPaymentService> _logger;
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly string _webhookSecret;

        // Razorpay orders expire after 15 minutes by default
        private static readonly TimeSpan OrderExpiry = TimeSpan.FromMinutes(15);

        public RazorpayPaymentService(
            IPaymentRepository paymentRepo,
            ISubscriptionService subscriptionService,
            IPlanService planService,
            IUserRepository userRepo,
            ILogger<RazorpayPaymentService> logger,
            IConfiguration config)
        {
            _paymentRepo = paymentRepo;
            _subscriptionService = subscriptionService;
            _planService = planService;
            _userRepo = userRepo;
            _logger = logger;
            _keyId = config["Razorpay:KeyId"] ?? throw new InvalidOperationException("Razorpay KeyId not configured");
            _keySecret = config["Razorpay:KeySecret"] ?? throw new InvalidOperationException("Razorpay KeySecret not configured");
            _webhookSecret = config["Razorpay:WebhookSecret"] ?? throw new InvalidOperationException("Razorpay WebhookSecret not configured");
        }

        // Fix #2: Amount is derived from the plan record, never from the client request.
        public async Task<CreateOrderResponse> CreateOrderAsync(long userId, Guid planId)
        {
            // Resolve authoritative price from the plan — never trust a client-supplied amount
            var plan = await _planService.GetPlanByIdAsync(planId);
            if (plan == null)
                throw new NotFoundException("Plan", planId.ToString());

            // Reuse a recent pending order to avoid duplicates (skip if expired)
            var existingPending = await _paymentRepo.GetPendingSubscriptionPaymentByUserIdAsync(userId);
            if (existingPending != null && existingPending.CreatedAt >= DateTime.UtcNow - OrderExpiry)
            {
                _logger.LogInformation("Reusing existing pending order {OrderId} for user {UserId}", existingPending.RazorpayOrderId, userId);
                return new CreateOrderResponse
                {
                    OrderId = existingPending.RazorpayOrderId!,
                    Amount = existingPending.Amount,
                    Currency = "INR",
                    KeyId = _keyId
                };
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId.ToString());

            var client = new RazorpayClient(_keyId, _keySecret);
            var serverAmount = plan.MonthlyAmount;

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(serverAmount * 100) },
                { "currency", "INR" },
                { "receipt", $"receipt_{userId}_{DateTime.UtcNow.Ticks}" }
            };

            // SDK call is synchronous — offload to avoid blocking a thread-pool thread
            var order = await Task.Run(() => client.Order.Create(options));
            // Explicit cast from dynamic to string — prevents dynamic dispatch errors on logger extension methods
            var razorpayOrderId = (string)order["id"];

            var payment = new Domain.Entities.Payment
            {
                PublicId = Guid.NewGuid(),
                SocietyId = user.SocietyId,
                Amount = serverAmount,
                ModeCode = PaymentModeCodes.Razorpay,
                // Encode planId into Reference so it can be resolved without guessing at verification
                Reference = $"plan:{planId}|order:{razorpayOrderId}",
                CreatedAt = DateTime.UtcNow,
                RazorpayOrderId = razorpayOrderId,
                PaymentType = PaymentTypeCodes.Subscription
            };

            await _paymentRepo.AddAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            _logger.LogInformation("Created Razorpay order {OrderId} for user {UserId}, plan {PlanId}, amount {Amount}",
                razorpayOrderId, userId, planId, serverAmount);

            return new CreateOrderResponse
            {
                OrderId = payment.RazorpayOrderId,
                Amount = payment.Amount,
                Currency = "INR",
                KeyId = _keyId
            };
        }

        public async Task<VerifyPaymentResponse> VerifyPaymentAsync(VerifyPaymentRequest request)
        {
            var payment = await _paymentRepo.GetByRazorpayOrderIdAsync(request.OrderId);
            if (payment == null)
            {
                _logger.LogWarning("VerifyPayment: order {OrderId} not found", request.OrderId);
                return new VerifyPaymentResponse { IsValid = false, Message = "Order not found" };
            }

            // Idempotency: already verified
            if (payment.RazorpayPaymentId != null)
            {
                _logger.LogInformation("VerifyPayment: order {OrderId} already verified", request.OrderId);
                return new VerifyPaymentResponse { IsValid = true, Message = "Payment already verified" };
            }

            // Fix #5: Use constant-time comparison to prevent timing attacks
            var expectedBytes = Encoding.UTF8.GetBytes(GenerateSignature(request.OrderId, request.PaymentId, _keySecret));
            var receivedBytes = Encoding.UTF8.GetBytes(request.Signature);
            var isSignatureValid = expectedBytes.Length == receivedBytes.Length
                                   && CryptographicOperations.FixedTimeEquals(expectedBytes, receivedBytes);

            if (!isSignatureValid)
            {
                // Fix #6: On failure, log only — never mutate DatePaid or store a forged signature
                _logger.LogWarning(
                    "VerifyPayment: invalid signature for order {OrderId}, paymentId {PaymentId}. Possible tampering attempt.",
                    request.OrderId, request.PaymentId);
                return new VerifyPaymentResponse { IsValid = false, Message = "Invalid signature" };
            }

            // Mark as verified — only reached after a valid signature
            payment.RazorpayPaymentId = request.PaymentId;
            payment.RazorpaySignature = request.Signature;
            payment.DatePaid = DateTime.UtcNow;
            payment.VerifiedAt = DateTime.UtcNow;

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            // Fix #2: Resolve plan from the payment record, not by position in a list
            await ActivateSubscriptionAsync(payment, request.PaymentId);

            _logger.LogInformation("Payment verified and subscription activated for order {OrderId}, paymentId {PaymentId}",
                request.OrderId, request.PaymentId);

            return new VerifyPaymentResponse { IsValid = true, Message = "Payment verified and subscription activated" };
        }

        // Fix #3 & #4: Accept raw body + signature; verify before processing; idempotency guard
        public async Task ProcessWebhookAsync(string rawBody, string signature, WebhookPayload payload)
        {
            // Fix #3: Verify Razorpay webhook signature before touching any data
            var expectedBytes = Encoding.UTF8.GetBytes(GenerateWebhookSignature(rawBody, _webhookSecret));
            var receivedBytes = Encoding.UTF8.GetBytes(signature);
            var isSignatureValid = expectedBytes.Length == receivedBytes.Length
                                   && CryptographicOperations.FixedTimeEquals(expectedBytes, receivedBytes);

            if (!isSignatureValid)
            {
                _logger.LogWarning("ProcessWebhook: invalid X-Razorpay-Signature. Possible spoofed webhook.");
                return; // Return 200 to Razorpay so it does not retry; we've rejected the payload silently
            }

            if (payload.Event != "payment.captured")
            {
                _logger.LogInformation("ProcessWebhook: ignoring unhandled event '{Event}'", payload.Event);
                return;
            }

            var paymentId = payload.Payment?.Id;
            var orderId = payload.Payment?.OrderId;

            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(orderId))
            {
                _logger.LogWarning("ProcessWebhook: missing paymentId or orderId in payload");
                return;
            }

            // Fix #4: Double idempotency guard — check both by orderId and by paymentId
            var existingByPaymentId = await _paymentRepo.GetByRazorpayPaymentIdAsync(paymentId);
            if (existingByPaymentId != null)
            {
                _logger.LogInformation("ProcessWebhook: duplicate webhook for paymentId {PaymentId}, skipping", paymentId);
                return;
            }

            var payment = await _paymentRepo.GetByRazorpayOrderIdAsync(orderId);
            if (payment == null)
            {
                _logger.LogWarning("ProcessWebhook: no local payment record for orderId {OrderId}", orderId);
                return;
            }

            if (payment.RazorpayPaymentId != null)
            {
                _logger.LogInformation("ProcessWebhook: orderId {OrderId} already processed, skipping", orderId);
                return;
            }

            payment.RazorpayPaymentId = paymentId;
            payment.DatePaid = DateTime.UtcNow;
            payment.VerifiedAt = DateTime.UtcNow;

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            await ActivateSubscriptionAsync(payment, paymentId);

            _logger.LogInformation("ProcessWebhook: subscription activated for orderId {OrderId}, paymentId {PaymentId}",
                orderId, paymentId);
        }

        // Shared subscription activation logic — resolves plan from the stored Reference
        private async Task ActivateSubscriptionAsync(Domain.Entities.Payment payment, string paymentReference)
        {
            var planId = ParsePlanIdFromReference(payment.Reference);
            if (planId == null)
                throw new InvalidOperationException($"Cannot resolve plan from payment reference '{payment.Reference}'");

            var plan = await _planService.GetPlanByIdAsync(planId.Value);
            if (plan == null)
                throw new NotFoundException("Plan", planId.Value.ToString());

            await _subscriptionService.SubscribeAsync(payment.SocietyId, new Application.DTOs.Subscription.SubscribeRequest
            {
                PlanId = plan.Id,
                Amount = payment.Amount,
                PaymentMethod = "Razorpay",
                PaymentReference = paymentReference
            });
        }

        // Reference format: "plan:{guid}|order:{razorpayOrderId}"
        private static Guid? ParsePlanIdFromReference(string? reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var parts = reference.Split('|');
            foreach (var part in parts)
            {
                if (part.StartsWith("plan:", StringComparison.Ordinal) &&
                    Guid.TryParse(part["plan:".Length..], out var id))
                    return id;
            }
            return null;
        }

        // Fix #5: HMAC for payment signature (orderId|paymentId)
        private static string GenerateSignature(string orderId, string paymentId, string secret)
        {
            var data = $"{orderId}|{paymentId}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return BytesToHex(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        // Fix #3: HMAC for webhook signature (raw JSON body)
        private static string GenerateWebhookSignature(string rawBody, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return BytesToHex(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody)));
        }

        private static string BytesToHex(byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}