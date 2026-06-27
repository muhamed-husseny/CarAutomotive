namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id); // Primary key configuration

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)"); // Configure decimal precision for price as two decimal places

            builder.Property(p => p.StockCount)
                   .IsRequired();

            builder.Property(p => p.CreatedDate)
                   .IsRequired();

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
