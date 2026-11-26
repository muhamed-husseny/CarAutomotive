using AutoMapper;
using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Orders;
using CarAutomotive.Core.Enums;
using CarAutomotive.Core.Interfaces;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(Guid clientId, CreateOrderDto dto)
        {
            if (dto == null || dto.Items == null || !dto.Items.Any())
            {
                throw new ArgumentException("Order must contain at least one item.");
            }

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = new List<Product>();

            foreach (var pid in productIds)
            {
                var product = await _unitOfWork.Repository<Product>().GetById(pid);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID '{pid}' was not found.");
                }
                products.Add(product);
            }

            // Validate stock availability
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product '{product.Title}'. Available: {product.StockQuantity}, Requested: {item.Quantity}.");
                }
            }

            // Calculate SubtotalEgp based on product BasePriceEgp * quantity
            decimal subtotal = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                var unitPrice = product.BasePriceEgp;
                var itemTotal = unitPrice * item.Quantity;
                subtotal += itemTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Title,
                    ImageUrl = product.ProductImages.FirstOrDefault()?.ImageUrl,
                    Price = unitPrice,
                    UnitPriceEgp = unitPrice,
                    Quantity = item.Quantity,
                    TotalPriceEgp = itemTotal,
                    MerchantId = product.MerchantId
                });

                // Decrement stock
                product.StockQuantity -= item.Quantity;
            }

            // Calculate PlatformFeeEgp as 15% of Subtotal (Subtotal * 0.15)
            decimal platformFee = Math.Round(subtotal * 0.15m, 2);

            // Determine Delivery Fee
            decimal deliveryFee = dto.DeliveryType.Equals("WORKSHOP_INSTALLATION", StringComparison.OrdinalIgnoreCase) 
                ? 0m 
                : 50.0m;

            // Set TotalAmountEgp = Subtotal + DeliveryFee
            decimal totalAmount = subtotal + deliveryFee;

            // Generate a unique OrderNumber (ORD-XXXXXX)
            string orderNumber = $"ORD-{Random.Shared.Next(100000, 999999)}";

            var shippingAddress = dto.ShippingAddress != null
                ? new ShippingAddress(
                    dto.ShippingAddress.FullName ?? string.Empty,
                    dto.ShippingAddress.PhoneNumber ?? string.Empty,
                    dto.ShippingAddress.City,
                    dto.ShippingAddress.Street,
                    dto.ShippingAddress.Governorate)
                : new ShippingAddress();

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderNumber = orderNumber,
                UserId = clientId,
                DeliveryType = dto.DeliveryType.ToUpperInvariant(),
                PartnerWorkshopId = dto.PartnerWorkshopId,
                ScheduledInstallationTime = dto.ScheduledInstallationTime,
                ShippingAddress = shippingAddress,
                SubtotalEgp = subtotal,
                DeliveryFeeEgp = deliveryFee,
                PlatformFeeEgp = platformFee,
                TotalAmountEgp = totalAmount,
                TotalAmount = totalAmount,
                FulfillmentStatus = "PENDING",
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Items = orderItems
            };

            _unitOfWork.Repository<Order>().Add(order);
            await _unitOfWork.CompleteAsync();

            return MapToOrderResponseDto(order);
        }

        public async Task<IReadOnlyList<MerchantOrderDto>> GetMerchantOrdersAsync(Guid merchantId)
        {
            var spec = new MerchantOrdersSpecification(merchantId);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            var merchantOrders = new List<MerchantOrderDto>();

            foreach (var order in orders)
            {
                var relevantItems = order.Items
                    .Where(i => i.MerchantId == merchantId)
                    .Select(i => new MerchantOrderItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        ImageUrl = i.ImageUrl,
                        UnitPriceEGP = i.UnitPriceEgp > 0 ? i.UnitPriceEgp : i.Price,
                        Quantity = i.Quantity,
                        TotalPriceEGP = i.TotalPriceEgp > 0 ? i.TotalPriceEgp : (i.Price * i.Quantity)
                    }).ToList();

                if (!relevantItems.Any())
                    continue;

                decimal itemsTotal = relevantItems.Sum(i => i.TotalPriceEGP);
                // Payout is items total minus 15% platform fee
                decimal netPayout = Math.Round(itemsTotal * 0.85m, 2);

                merchantOrders.Add(new MerchantOrderDto
                {
                    OrderId = order.OrderId,
                    OrderNumber = order.OrderNumber,
                    FulfillmentStatus = order.FulfillmentStatus,
                    CreatedAt = order.CreatedAt,
                    DeliveryType = order.DeliveryType,
                    PartnerWorkshopId = order.PartnerWorkshopId,
                    ScheduledInstallationTime = order.ScheduledInstallationTime,
                    MerchantPayoutEGP = netPayout,
                    ShippingAddress = order.ShippingAddress != null ? new OrderShippingAddressDto
                    {
                        Street = order.ShippingAddress.Street,
                        City = order.ShippingAddress.City,
                        Governorate = order.ShippingAddress.Governorate ?? string.Empty,
                        FullName = order.ShippingAddress.FullName,
                        PhoneNumber = order.ShippingAddress.PhoneNumber
                    } : null,
                    Items = relevantItems
                });
            }

            return merchantOrders;
        }

        public async Task<bool> UpdateFulfillmentStatusAsync(Guid merchantId, Guid orderId, string status)
        {
            var spec = new OrderByIdWithItemsSpecification(orderId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (order == null)
                return false;

            // Ensure the order contains items belonging to this merchantId
            bool containsMerchantItems = order.Items.Any(i => i.MerchantId == merchantId);
            if (!containsMerchantItems)
                return false;

            var normalizedStatus = status.ToUpperInvariant();
            order.FulfillmentStatus = normalizedStatus;

            // Sync with legacy OrderStatus enum if applicable
            switch (normalizedStatus)
            {
                case "PROCESSING":
                    order.Status = OrderStatus.Processing;
                    break;
                case "DISPATCHED":
                case "SHIPPED":
                    order.Status = OrderStatus.Shipped;
                    break;
                case "DELIVERED":
                case "AT_WORKSHOP":
                    order.Status = OrderStatus.Delivered;
                    break;
                case "CANCELLED":
                    order.Status = OrderStatus.Cancelled;
                    break;
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IReadOnlyList<OrderResponseDto>> GetOrdersForUserAsync(Guid userId, string? status = null)
        {
            OrderStatus? enumStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsed))
            {
                enumStatus = parsed;
            }

            var spec = new OrderWithItemsSpecification(userId, enumStatus);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders.Select(MapToOrderResponseDto).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            var spec = new OrderWithItemsSpecification(orderId, userId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            return order == null ? null : MapToOrderResponseDto(order);
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, Guid userId)
        {
            var spec = new OrderWithItemsSpecification(orderId, userId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (order == null)
                return false;

            if (order.FulfillmentStatus == "DISPATCHED" ||
                order.FulfillmentStatus == "DELIVERED" ||
                order.FulfillmentStatus == "CANCELLED" ||
                order.Status == OrderStatus.Shipped ||
                order.Status == OrderStatus.Delivered ||
                order.Status == OrderStatus.Cancelled)
            {
                return false;
            }

            // Restore product stock
            foreach (var item in order.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetById(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            order.FulfillmentStatus = "CANCELLED";
            order.Status = OrderStatus.Cancelled;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        private static OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderNumber = !string.IsNullOrEmpty(order.OrderNumber) ? order.OrderNumber : $"ORD-{order.OrderId.ToString("N")[..8].ToUpper()}",
                FulfillmentStatus = !string.IsNullOrEmpty(order.FulfillmentStatus) ? order.FulfillmentStatus : order.Status.ToString().ToUpperInvariant(),
                SubtotalEGP = order.SubtotalEgp > 0 ? order.SubtotalEgp : order.TotalAmount,
                DeliveryFeeEGP = order.DeliveryFeeEgp,
                PlatformFeeEGP = order.PlatformFeeEgp,
                TotalAmountEGP = order.TotalAmountEgp > 0 ? order.TotalAmountEgp : order.TotalAmount,
                CreatedAt = order.CreatedAt != default ? order.CreatedAt : order.OrderDate,
                DeliveryType = order.DeliveryType,
                PartnerWorkshopId = order.PartnerWorkshopId,
                ScheduledInstallationTime = order.ScheduledInstallationTime,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ImageUrl = i.ImageUrl,
                    UnitPriceEGP = i.UnitPriceEgp > 0 ? i.UnitPriceEgp : i.Price,
                    Quantity = i.Quantity,
                    TotalPriceEGP = i.TotalPriceEgp > 0 ? i.TotalPriceEgp : (i.Price * i.Quantity)
                }).ToList()
            };
        }
    }
}
