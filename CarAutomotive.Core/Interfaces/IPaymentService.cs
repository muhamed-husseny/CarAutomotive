namespace CarAutomotive.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreateOrUpdatePaymentIntent(Guid orderId);
        Task<Payment> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Payment> UpdateOrderPaymentFailed(string paymentIntentId);

    }
}
