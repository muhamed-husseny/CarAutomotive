namespace CarAutomotive.Application.Dtos
{
    public class AddToCartDto
    {
       public int ProductId { get; set; }
       public int Quantity { get; set; }
        
        //this dto contain prodcutID & Quantity only casue the user allowed to send only these two properties
    }
}
