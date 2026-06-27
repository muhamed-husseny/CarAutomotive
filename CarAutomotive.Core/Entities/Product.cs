namespace CarAutomotive.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockCount { get; set; }

        public DateTime CreatedDate { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;


        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;


        public ICollection<ProductImage> ProductImages
        { get; set; } = new HashSet<ProductImage>();


        public ICollection<Compatibility> Compatibilities
        { get; set; } = new HashSet<Compatibility>();
    }
}