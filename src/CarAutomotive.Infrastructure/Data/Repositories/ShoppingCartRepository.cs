using CarAutomotive.Core.Entities;
using StackExchange.Redis;
namespace CarAutomotive.Infrastructure.Data.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IDatabase _database;
        private static string GetCartKey(string cartId)
        {
            return $"cart:{cartId}"; // We use this prefix to avoid key collisions in Redis and to make it easier to manage the keys related to shopping carts => mohm yasta
        }
        public ShoppingCartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<bool> DeleteShoppingCartAsync(string cartId)
        {
            return await _database.KeyDeleteAsync(GetCartKey(cartId));
        }
        public async Task<ShoppingCart?> GetShoppingCartAsync(string cartId)
        {
            var cart = await _database.StringGetAsync(GetCartKey(cartId));
            return cart.IsNullOrEmpty? null: JsonSerializer.Deserialize<ShoppingCart>(cart!);
        }
        public async Task<ShoppingCart?> UpdateShoppingCartAsync(ShoppingCart cart)
        {
            var createdOrUpdated = await _database.StringSetAsync(GetCartKey(cart.Id),JsonSerializer.Serialize(cart),TimeSpan.FromDays(30));
            return createdOrUpdated? await GetShoppingCartAsync(cart.Id):null;
        }
    }
}