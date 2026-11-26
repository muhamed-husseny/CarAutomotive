namespace CarAutomotive.Application.Dtos.Cart
{
    public class UpdateCartItemDto
    {
        public Guid ProductId { get; set; }
        public int NewQuantity { get; set; }
    }
}