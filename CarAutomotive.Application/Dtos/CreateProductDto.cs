namespace CarAutomotive.Application.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new(); // image urls not product images because we will create product images from these urls in the service layer Rkz 4woya
    }
}
