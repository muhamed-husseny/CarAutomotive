namespace CarAutomotive.Application.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(string cartId);
        Task<CartDto?> AddToCartAsync(string cartId, AddToCartDto dto);
        Task<CartDto?> UpdateQuantityAsync(string cartId, UpdateCartItemDto dto);
        Task<CartDto?> RemoveFromCartAsync(string cartId, int productId);
        Task<bool> ClearCartAsync(string cartId);
    }
}