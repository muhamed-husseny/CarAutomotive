using Stripe;

namespace CarAutomotive.Infrastructure.Data.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _dbContext;

        public StripePaymentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Payment> CreateOrUpdatePaymentIntent(Guid orderId)
        {
            
            long amountInCents = 500 * 100;

            var service = new PaymentIntentService();
            PaymentIntent intent;

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "egp", 
                PaymentMethodTypes = new List<string> { "card" }
            };

            intent = await service.CreateAsync(options);

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = 500, 
                PaymentIntentId = intent.Id,
                ClientSecret = intent.ClientSecret,
                Status = Core.Enums.PaymentStatus.Pending 
            };

            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);

            if (payment == null) return null;

            
            payment.Status = Core.Enums.PaymentStatus.Succeeded;

            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();

            return payment;
        }
        public async Task<Payment> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);

            if (payment == null) return null;

          
            payment.Status = Core.Enums.PaymentStatus.Failed;

           

            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();

            return payment;
        }


    }
}
