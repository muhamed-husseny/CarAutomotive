namespace CarAutomotive.Core.Entities.Orders
{
    public class ShippingAddress
    {
        public ShippingAddress()
        {
        }

        public ShippingAddress(string fullName, string phoneNumber, string city, string street, string? governorate = null)
        {
            FullName = fullName ?? string.Empty;
            PhoneNumber = phoneNumber ?? string.Empty;
            City = city ?? string.Empty;
            Street = street ?? string.Empty;
            Governorate = governorate;
        }

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string? Governorate { get; set; }
    }
}