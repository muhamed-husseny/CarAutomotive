namespace CarAutomotive.Core.Entities.Orders
{
    public class OrderItem 
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPriceEgp { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPriceEgp { get; set; }
        public Guid? MerchantId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}