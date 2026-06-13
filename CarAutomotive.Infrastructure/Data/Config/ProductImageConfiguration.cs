using CarAutomotive.Core.Entities;
namespace CarAutomotive.Infrastructure.Data.Config
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(p=>p.Id);
            builder.Property(p=>p.ImageUrl).IsRequired()
                                            .HasMaxLength(500);
            builder.HasOne(p => p.Product)
                   .WithMany(p => p.ProductImages)
                   .HasForeignKey(p => p.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
