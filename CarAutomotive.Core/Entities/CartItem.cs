namespace CarAutomotive.Core.Entities
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = null!;
        public int Quantity { get; set; } = 1;

    }
}