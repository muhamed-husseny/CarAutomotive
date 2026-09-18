using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.Core.Specifications
{
    public class MerchantOrdersSpecification : BaseSpecification<Order>
    {
        public MerchantOrdersSpecification(Guid merchantId)
            : base(o => o.Items.Any(i => i.MerchantId == merchantId))
        {
            AddInclude(o => o.Items);
            AddOrderByDescending(o => o.CreatedAt);
        }
    }
}
