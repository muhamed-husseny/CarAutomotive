using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Entities.Payments;

namespace CarAutomotive.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponseDto> CreateOrUpdatePaymentIntentAsync(Guid orderId);
        Task<bool> HandlePaymentSuccessWebhookAsync(string paymentIntentId);
        Task<Payment> CreateOrUpdatePaymentIntent(Guid orderId);
        Task<Payment> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Payment> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}
