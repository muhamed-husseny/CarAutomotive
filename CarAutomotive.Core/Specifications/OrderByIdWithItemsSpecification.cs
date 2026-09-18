using CarAutomotive.Core.Entities.Orders;

namespace CarAutomotive.Core.Specifications
{
    public class OrderByIdWithItemsSpecification : BaseSpecification<Order>
    {
        public OrderByIdWithItemsSpecification(Guid orderId)
            : base(o => o.OrderId == orderId)
        {
            AddInclude(o => o.Items);
        }
    }
}
