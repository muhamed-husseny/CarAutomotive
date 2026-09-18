using CarAutomotive.Core.Entities.Payments;

namespace CarAutomotive.Core.Specifications
{
    public class PaymentByOrderIdSpecification : BaseSpecification<Payment>
    {
        public PaymentByOrderIdSpecification(Guid orderId)
            : base(p => p.OrderId == orderId)
        {
        }
    }

    public class PaymentByIntentIdSpecification : BaseSpecification<Payment>
    {
        public PaymentByIntentIdSpecification(string paymentIntentId)
            : base(p => p.PaymentIntentId == paymentIntentId)
        {
        }
    }
}
