using SocietyLedger.Application.DTOs.Razorpay;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IRazorpayPaymentService
    {
        /// <summary>Creates a Razorpay order. Amount is resolved server-side from planId.</summary>
        Task<CreateOrderResponse> CreateOrderAsync(long userId, Guid planId);
        Task<VerifyPaymentResponse> VerifyPaymentAsync(VerifyPaymentRequest request);
        /// <summary>Processes a Razorpay webhook event after verifying the HMAC-SHA256 signature.</summary>
        Task ProcessWebhookAsync(string rawBody, string signature, WebhookPayload payload);
    }
}