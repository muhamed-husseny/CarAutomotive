namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasOne(v => v.User)
                   .WithMany(u => u.Vehicles)
                   .HasForeignKey(v => v.UserId);

            builder.Property(v => v.Make)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.Model)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(v => v.Year)
                   .IsRequired();

        }
    }
}
