namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasOne(v => v.AppUser)
                   .WithMany(u => u.Vehicles)
                   .HasForeignKey(v => v.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(v => v.Make)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(v => v.Model)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(v => v.Year)
                   .IsRequired();

            builder.Property(v => v.PlateCode)
                   .HasMaxLength(50);

            builder.Property(v => v.PlateNumber)
                   .HasMaxLength(50);

            builder.Property(v => v.Mileage)
                   .IsRequired();

            builder.Property(v => v.FuelType)
                   .HasMaxLength(50);

            builder.Property(v => v.Color)
                   .HasMaxLength(50);

            builder.Property(v => v.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(v => v.Vin)
                   .HasMaxLength(17);

            builder.Property(v => v.ImageUrl)
                   .HasMaxLength(500);
        }
    }
}
