
namespace CarAutomotive.Infrastructure.Data.Config
{
    public class MechanicServiceConfiguration : IEntityTypeConfiguration<MechanicService>
    {
        public void Configure(EntityTypeBuilder<MechanicService> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ServiceName).IsRequired().HasMaxLength(100);

            builder.Property(s => s.EstimatedPrice)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
