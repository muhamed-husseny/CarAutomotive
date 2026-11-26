namespace CarAutomotive.Application.Dtos.Orders
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}