namespace CarAutomotive.Core.Entities.Orders
{
    public class Order : BaseEntity
    {
        public Order()
        {
            
        }
        public Order(Guid userId,ShippingAddress shippingAddress,decimal totalAmount, ICollection<OrderItem> items)
        {
            UserId = userId;
            ShippingAddress = shippingAddress;
            TotalAmount = totalAmount;
            Items = items;
            OrderDate = DateTime.UtcNow;
        }

        public Guid UserId { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> Items { get; set; }
            = new HashSet<OrderItem>();
    }
}