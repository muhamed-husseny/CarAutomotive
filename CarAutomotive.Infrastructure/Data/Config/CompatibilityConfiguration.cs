namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class CompatibilityConfiguration : IEntityTypeConfiguration<Compatibility>
    {
        public void Configure(EntityTypeBuilder<Compatibility> builder)
        {
            builder.HasOne(c => c.Product)
                   .WithMany(p => p.Compatibilities)
                   .HasForeignKey(c => c.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.Make)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(c => c.Model)
                   .HasMaxLength(100)
                   .IsRequired();
            builder.Property(c => c.Year)
                    .IsRequired();
            builder.HasIndex(c => new
            {
                c.ProductId,
                c.Make,
                c.Model,
                c.Year
            }).IsUnique();
        }
    }
}
