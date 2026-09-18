using CarAutomotive.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAutomotive.Infrastructure.Data.Config
{
    internal class WalletLedgerConfiguration : IEntityTypeConfiguration<WalletLedger>
    {
        public void Configure(EntityTypeBuilder<WalletLedger> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.UserId)
                   .IsRequired();

            builder.HasOne(w => w.User)
                   .WithMany()
                   .HasForeignKey(w => w.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(w => w.ReferenceType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(w => w.ReferenceId)
                   .IsRequired();

            builder.Property(w => w.Type)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(w => w.AmountEgp)
                   .HasColumnType("decimal(12,2)")
                   .IsRequired();

            builder.Property(w => w.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(w => w.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(w => w.CreatedAt)
                   .IsRequired();
        }
    }
}
