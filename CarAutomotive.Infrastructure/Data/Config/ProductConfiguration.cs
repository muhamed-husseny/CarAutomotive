namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id); // Primary key configuration

            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.BasePriceEgp)
                   .HasColumnType("decimal(12,2)")
                   .IsRequired();

            builder.Property(p => p.SalePriceEgp)
                   .HasColumnType("decimal(12,2)");

            builder.Property(p => p.ItemCostEgp)
                   .HasColumnType("decimal(12,2)");

            builder.Property(p => p.StockQuantity)
                   .IsRequired();

            builder.Property(p => p.CreatedDate)
                   .IsRequired();

            builder.Property(p => p.MerchantId);

            builder.Property(p => p.OuterDiameterMM)
                   .HasColumnType("decimal(12,2)");

            builder.Property(p => p.ThicknessMM)
                   .HasColumnType("decimal(12,2)");

            builder.Property(p => p.BoltHoleCircleMM)
                   .HasColumnType("decimal(12,2)");

            builder.Property(p => p.LengthMM)
                   .HasColumnType("decimal(12,2)");

            builder.Property(p => p.ThreadSize)
                   .HasMaxLength(50);

            builder.Property(p => p.VinCompatibilityPattern)
                   .HasMaxLength(500);

            builder.Property(p => p.IsVisible)
                   .HasDefaultValue(true);

            builder.Property(p => p.WorkshopDeliveryEnabled)
                   .HasDefaultValue(true);

            builder.HasOne(p => p.Category) // Configure the relationship between Product and Category
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId) // Set the foreign key for the relationship
                   .OnDelete(DeleteBehavior.Restrict); // Configure delete behavior to restrict deletion of a category if it has related products

            builder.HasOne(p => p.Brand)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
