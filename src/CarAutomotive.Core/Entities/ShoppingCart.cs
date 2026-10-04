namespace CarAutomotive.Core.Entities
{
    public class ShoppingCart
    {
        public ShoppingCart(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }
}