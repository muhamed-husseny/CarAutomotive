namespace CarAutomotive.Core.Entities
{
    public class ShoppingCart
    {
        // we didnt inherit from BaseEntity because we won't insert this entity into the database (we are not deadling with store context we are dealing with redis DB), it will be stored in Redis as a string, so we will receive it from the frontend as a GUID and store it as a string
        public string Id {  get; set; } = null!; // will receive it from Frontend as GUID so it will be stored as string
        public List<CartItem> Items { get; set; } = new(); 
    }
}
