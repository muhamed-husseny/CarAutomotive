namespace CarAutomotive.Core.Entities.Orders
{
    public class Order : BaseEntity
    {
        public Order()
        {
            OrderId = Guid.NewGuid();
            OrderDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public Order(Guid userId, ShippingAddress shippingAddress, decimal totalAmount, ICollection<OrderItem> items)
        {
            OrderId = Guid.NewGuid();
            UserId = userId;
            ShippingAddress = shippingAddress;
            TotalAmount = totalAmount;
            TotalAmountEgp = totalAmount;
            SubtotalEgp = totalAmount;
            Items = items;
            OrderDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid OrderId { get; set; } = Guid.NewGuid();
        public string OrderNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string DeliveryType { get; set; } = "DIRECT_DELIVERY"; // DIRECT_DELIVERY or WORKSHOP_INSTALLATION
        public Guid? PartnerWorkshopId { get; set; }
        public DateTime? ScheduledInstallationTime { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = new ShippingAddress();
        public decimal SubtotalEgp { get; set; }
        public decimal DeliveryFeeEgp { get; set; }
        public decimal PlatformFeeEgp { get; set; }
        public decimal TotalAmountEgp { get; set; }
        public decimal TotalAmount { get; set; }
        public string FulfillmentStatus { get; set; } = "PENDING"; // PENDING, PROCESSING, DISPATCHED, DELIVERED, AT_WORKSHOP, CANCELLED
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; }
            = new HashSet<OrderItem>();
    }
}