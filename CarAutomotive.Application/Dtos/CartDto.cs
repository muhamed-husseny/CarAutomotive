namespace CarAutomotive.Application.Dtos.Cart
{
    public class CartDto
    {
        public string Id { get; set; } = null!;
        public IReadOnlyList<CartItemDto> Items { get; set;}= new List<CartItemDto>();
        public int TotalQuantity => Items.Sum(item => item.Quantity);
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}