namespace CarAutomotive.Core.Specifications
{
    public class OrderWithItemsSpecification : BaseSpecification<Order>
    {
        public OrderWithItemsSpecification(Guid userId)
            : base(o => o.UserId == userId)
        {
            AddInclude(o => o.Items);

            AddOrderByDescending(o => o.OrderDate);
        }

        public OrderWithItemsSpecification(
            Guid userId,
            OrderStatus? status)
            : base(o =>
                o.UserId == userId &&
                (!status.HasValue || o.Status == status.Value))
        {
            AddInclude(o => o.Items);

            AddOrderByDescending(o => o.OrderDate);
        }

        public OrderWithItemsSpecification(
            Guid orderId,
            Guid userId)
            : base(o => o.OrderId == orderId && o.UserId == userId)
        {
            AddInclude(o => o.Items);
        }
    }
}