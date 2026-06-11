using CarAutomotive.Core.Entities.Orders;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(
            IShoppingCartRepository cartRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderToReturnDto?> CreateOrderAsync(Guid userId,CreateOrderDto dto)
        {
            var cart = await _cartRepository.GetShoppingCartAsync(dto.CartId);

            if (cart is null || !cart.Items.Any())
                return null;

            var productIds = cart.Items
                .Select(i => i.ProductId)
                .ToList();

            var spec = new ProductsForOrderSpecification(productIds);

            var products = await _unitOfWork
                .Repository<Product>()
                .GetAllWithSpecTrackedAsync(spec);

            foreach (var cartItem in cart.Items)
            {
                var product = products.FirstOrDefault(
                    p => p.Id == cartItem.ProductId);

                if (product is null)
                    return null;

                if (cartItem.Quantity > product.StockCount)
                    return null;
            }

            var orderItems = cart.Items.Select(cartItem =>
            {
                var product = products.First(
                    p => p.Id == cartItem.ProductId);

                return new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ProductImages.FirstOrDefault()?.ImageUrl,
                    Price = product.Price,
                    Quantity = cartItem.Quantity
                };
            }).ToList();

            var totalAmount = orderItems.Sum(
                item => item.Price * item.Quantity);

            var order = new Order(
                userId,
                new ShippingAddress(
                    dto.ShippingAddress.FullName,
                    dto.ShippingAddress.PhoneNumber,
                    dto.ShippingAddress.City,
                    dto.ShippingAddress.Street),
                totalAmount,
                orderItems);

            _unitOfWork
                .Repository<Order>()
                .Add(order);

            foreach (var product in products)
            {
                var cartItem = cart.Items.First(
                    i => i.ProductId == product.Id);

                product.StockCount -= cartItem.Quantity;
            }


            await _unitOfWork.CompleteAsync();

            await _cartRepository.DeleteShoppingCartAsync(dto.CartId);

            return _mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(Guid userId,OrderStatus? status)
        {
            var spec = new OrderWithItemsSpecification(
                userId,
                status);

            var orders = await _unitOfWork
                .Repository<Order>()
                .GetAllWithSpecAsync(spec);

            return _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
        }

        public async Task<OrderToReturnDto?> GetOrderByIdAsync(int orderId,Guid userId)
        {
            var spec = new OrderWithItemsSpecification(
                orderId,
                userId);

            var order = await _unitOfWork
                .Repository<Order>()
                .GetByIdWithSpecAsync(spec);

            if (order is null)
                return null;

            return _mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<bool> CancelOrderAsync(int orderId,Guid userId)
        {
            var spec = new OrderWithItemsSpecification(
                orderId,
                userId);

            var order = await _unitOfWork
                .Repository<Order>()
                .GetByIdWithSpecAsync(spec);

            if (order is null)
                return false;

            if (order.Status == OrderStatus.Shipped ||
                order.Status == OrderStatus.Delivered ||
                order.Status == OrderStatus.Cancelled)
                return false;

            foreach (var item in order.Items)
            {
                var product = await _unitOfWork
                    .Repository<Product>()
                    .GetByIdAsync(item.ProductId);

                if (product is not null)
                {
                    product.StockCount += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}