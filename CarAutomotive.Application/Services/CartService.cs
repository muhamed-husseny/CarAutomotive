using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IShoppingCartRepository cartRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto?> GetCartAsync(string cartId)
        {
            var cart = await _cartRepository.GetShoppingCartAsync(cartId);
            if (cart is null) return null;
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto?> AddToCartAsync(string cartId, AddToCartDto dto)
        {
            var spec = new ProductsWithCategorySpec(dto.ProductId);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
            if (product is null) return null;

            var cart = await _cartRepository.GetShoppingCartAsync(cartId)
                       ?? new ShoppingCart(cartId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem is not null)
                existingItem.Quantity += dto.Quantity;
            else
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = dto.Quantity,
                    CategoryName = product.Category.Name,
                    ImageUrl = product.ProductImages?.FirstOrDefault()?.ImageUrl ?? string.Empty
                });

            var updatedCart = await _cartRepository.UpdateShoppingCartAsync(cart);
            return _mapper.Map<CartDto>(updatedCart);
        }

        public async Task<CartDto?> UpdateQuantityAsync(string cartId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetShoppingCartAsync(cartId);
            if (cart is null) return null;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (item is null) return null;

            if (dto.NewQuantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = dto.NewQuantity;

            var updatedCart = await _cartRepository.UpdateShoppingCartAsync(cart);
            return _mapper.Map<CartDto>(updatedCart);
        }

        public async Task<CartDto?> RemoveFromCartAsync(string cartId, int productId)
        {
            var cart = await _cartRepository.GetShoppingCartAsync(cartId);
            if (cart is null) return null;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return null;

            cart.Items.Remove(item);

            var updatedCart = await _cartRepository.UpdateShoppingCartAsync(cart);
            return _mapper.Map<CartDto>(updatedCart);
        }

        public async Task<bool> ClearCartAsync(string cartId)
        {
            return await _cartRepository.DeleteShoppingCartAsync(cartId);
        }
    }
}