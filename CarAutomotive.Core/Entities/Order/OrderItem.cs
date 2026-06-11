namespace CarAutomotive.Core.Entities.Orders
{
    public class OrderItem : BaseEntity
    {
        
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}