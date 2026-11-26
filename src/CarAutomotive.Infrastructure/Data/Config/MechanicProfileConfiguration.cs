namespace CarAutomotive.Infrastructure.Data.Config
{
    public class MechanicProfileConfiguration : IEntityTypeConfiguration<MechanicProfile>
    {
        public void Configure(EntityTypeBuilder<MechanicProfile> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Location)
                   .HasColumnType("geography(Point, 4326)")
                   .IsRequired();

            builder.HasIndex(m => m.Location)
                   .HasMethod("GIST");


            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);

            builder.HasMany(m => m.Services)
                   .WithOne(s => s.MechanicProfile)
                   .HasForeignKey(s => s.MechanicProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
