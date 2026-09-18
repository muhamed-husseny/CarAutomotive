namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderId)
                   .IsRequired();

            builder.Property(o => o.OrderNumber)
                   .HasMaxLength(50);

            builder.Property(o => o.DeliveryType)
                   .HasMaxLength(50);

            builder.Property(o => o.FulfillmentStatus)
                   .HasMaxLength(50);

            builder.Property(o => o.SubtotalEgp)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DeliveryFeeEgp)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.PlatformFeeEgp)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TotalAmountEgp)
                   .HasColumnType("decimal(18,2)");

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
                       .HasMaxLength(100);

                address.Property(a => a.PhoneNumber)
                       .HasMaxLength(20);

                address.Property(a => a.City)
                       .HasMaxLength(100);

                address.Property(a => a.Street)
                       .HasMaxLength(200);

                address.Property(a => a.Governorate)
                       .HasMaxLength(100);
            });
        }
    }
}