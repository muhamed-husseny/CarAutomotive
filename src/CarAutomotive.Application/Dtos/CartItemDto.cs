namespace CarAutomotive.Application.Dtos.Cart
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity; // this property is calculated cause we dont want to store it in the database we can calculate it on the fly when we need it cause we can calculate it from the price and quantity
    }
}