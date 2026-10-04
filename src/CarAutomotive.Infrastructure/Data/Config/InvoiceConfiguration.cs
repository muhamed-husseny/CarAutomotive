using CarAutomotive.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAutomotive.Infrastructure.Data.Config
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.LaborTotalEgp)
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.PartsTotalEgp)
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.TotalAmountEgp)
                   .HasColumnType("decimal(18,2)");

            builder.HasMany(i => i.Parts)
                   .WithOne(p => p.Invoice)
                   .HasForeignKey(p => p.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class InvoicePartConfiguration : IEntityTypeConfiguration<InvoicePart>
    {
        public void Configure(EntityTypeBuilder<InvoicePart> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
