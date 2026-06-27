namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasOne(v => v.AppUser)
           .WithMany(u => u.Vehicles)
           .HasForeignKey(v => v.AppUserId);

            builder.Property(v => v.Make)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.Model)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(v => v.Year)
                   .IsRequired();

            builder.Property(v => v.PlateCode)
                   .HasMaxLength(50)
                   .IsRequired();


            builder.Property(v => v.PlateNumber)
                   .HasMaxLength(50)
                   .IsRequired();


            builder.Property(v => v.ImageUrl)
                   .HasMaxLength(500);


            builder.Property(v => v.Mileage)
                   .IsRequired();


        }
    }
}
