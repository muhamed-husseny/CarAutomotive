namespace CarAutomotive.Core.DTOs
{
    public class CreateOrderDto
    {
        public string DeliveryType { get; set; } = "DIRECT_DELIVERY"; // "DIRECT_DELIVERY" or "WORKSHOP_INSTALLATION"
        public Guid? PartnerWorkshopId { get; set; }
        public DateTime? ScheduledInstallationTime { get; set; }
        public OrderShippingAddressDto? ShippingAddress { get; set; }
        public List<OrderItemCreationDto> Items { get; set; } = new();
    }

    public class OrderShippingAddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class OrderItemCreationDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string FulfillmentStatus { get; set; } = string.Empty;
        public decimal SubtotalEGP { get; set; }
        public decimal DeliveryFeeEGP { get; set; }
        public decimal PlatformFeeEGP { get; set; }
        public decimal TotalAmountEGP { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryType { get; set; } = string.Empty;
        public Guid? PartnerWorkshopId { get; set; }
        public DateTime? ScheduledInstallationTime { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal UnitPriceEGP { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPriceEGP { get; set; }
    }

    public class MerchantOrderDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string FulfillmentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string DeliveryType { get; set; } = string.Empty;
        public Guid? PartnerWorkshopId { get; set; }
        public DateTime? ScheduledInstallationTime { get; set; }
        public decimal MerchantPayoutEGP { get; set; }
        public OrderShippingAddressDto? ShippingAddress { get; set; }
        public List<MerchantOrderItemDto> Items { get; set; } = new();
    }

    public class MerchantOrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal UnitPriceEGP { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPriceEGP { get; set; }
    }

    public class UpdateFulfillmentStatusDto
    {
        public string FulfillmentStatus { get; set; } = string.Empty;
    }
}
