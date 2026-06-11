namespace CarAutomotive.Application.Dtos.Orders
{
    public class ShippingAddressDto
    {
        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Street { get; set; } = null!;
    }
}