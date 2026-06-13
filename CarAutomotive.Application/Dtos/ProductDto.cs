namespace CarAutomotive.Application.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; } 
        public String CategoryName { get; set; } = null!; 
        public List<String> ProductImages { get; set; } = new (); 
    }
}
