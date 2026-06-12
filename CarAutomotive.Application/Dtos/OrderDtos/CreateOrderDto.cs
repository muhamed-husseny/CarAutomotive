namespace CarAutomotive.Application.Dtos.Orders
{
    public class CreateOrderDto
    {
        public string CartId { get; set; } = null!;

        public ShippingAddressDto ShippingAddress { get; set; } = null!;
    }
}