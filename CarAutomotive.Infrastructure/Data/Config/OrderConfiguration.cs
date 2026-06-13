namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                     .IsRequired();

            builder.Property(o => o.OrderDate)
                    .IsRequired();

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .IsRequired();


            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.FullName)
                       .HasMaxLength(100)
                       .IsRequired();

                address.Property(a => a.PhoneNumber)
                       .HasMaxLength(20)
                       .IsRequired();

                address.Property(a => a.City)
                       .HasMaxLength(100)
                       .IsRequired();

                address.Property(a => a.Street)
                       .HasMaxLength(200)
                       .IsRequired();
            });
        }
    }
}