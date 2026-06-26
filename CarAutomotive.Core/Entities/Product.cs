namespace CarAutomotive.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; } // Fk TO Category 
        public Category Category { get; set; } = null!; // Navigation property for related Category
        public ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>(); // Navigation property for related ProductImages 

        public int BrandId { get; set; } // Fk TO Brand
        public Brand Brand { get; set; } = null!; // Navigation property for related Brand
    }
}
