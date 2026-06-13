namespace CarAutomotive.Core.Entities.Orders
{
    public class ShippingAddress
    {
        public ShippingAddress()
        {
            
        }
        public ShippingAddress(string fullName, string phoneNumber, string city, string street)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            City = city;
            Street = street;
        }

        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
    }
}