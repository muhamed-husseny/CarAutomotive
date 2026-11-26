using System.ComponentModel.DataAnnotations.Schema;

namespace CarAutomotive.Core.Entities
{
    public class Product : BaseEntity<Guid>
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public decimal BasePriceEgp { get; set; }
        public decimal? SalePriceEgp { get; set; }
        public decimal? ItemCostEgp { get; set; }

        public int StockQuantity { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? MerchantId { get; set; }

        public decimal? OuterDiameterMM { get; set; }
        public decimal? ThicknessMM { get; set; }
        public decimal? BoltHoleCircleMM { get; set; }
        public decimal? LengthMM { get; set; }
        public string? ThreadSize { get; set; }
        public string? VinCompatibilityPattern { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool WorkshopDeliveryEnabled { get; set; } = true;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();

        public ICollection<Compatibility> Compatibilities { get; set; } = new HashSet<Compatibility>();

        // Backward compatibility helpers
        [NotMapped]
        public string Name
        {
            get => Title;
            set => Title = value;
        }

        [NotMapped]
        public decimal Price
        {
            get => BasePriceEgp;
            set => BasePriceEgp = value;
        }

        [NotMapped]
        public int StockCount
        {
            get => StockQuantity;
            set => StockQuantity = value;
        }
    }
}