namespace CarAutomotive.Application.Dtos.Orders
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public ShippingAddressDto ShippingAddress { get; set; } = null!;

        public IReadOnlyList<OrderItemDto> Items { get; set; }
            = new List<OrderItemDto>();
    }
}