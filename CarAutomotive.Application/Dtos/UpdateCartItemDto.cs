namespace CarAutomotive.Application.Dtos.Cart
{
    public class UpdateCartItemDto
    {
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
    }
}