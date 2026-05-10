namespace CarAutomotive.Infrastructure.Data.Config
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasOne(a => a.User)
                   .WithMany()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Mechanic)
                   .WithMany()
                   .HasForeignKey(a => a.MechanicId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(a => a.Notes).HasMaxLength(500);

            builder.Property(a => a.Status)
                   .HasConversion<int>();

        }
    }
}
