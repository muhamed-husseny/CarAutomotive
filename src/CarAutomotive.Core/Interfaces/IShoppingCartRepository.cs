using CarAutomotive.Core.Entities;

namespace CarAutomotive.Core.Interfaces
{
    public interface IShoppingCartRepository
    {
         Task<ShoppingCart?> GetShoppingCartAsync(string cartId);
         Task<ShoppingCart?> UpdateShoppingCartAsync(ShoppingCart cart); // Update or create a shopping cart cause it can be used for both cause if the cart doesn't exist it will be created and if it exist it will be updated cause it stored from redis not from database and thats how redis work it will update the value if it exist and if it doesn't exist it will create a new one rkz yasta
        Task<bool> DeleteShoppingCartAsync(string cartId);
    }
}
