using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class LocationPaymentsConfiguration : IEntityTypeConfiguration<LocationPayments>
    {
        public void Configure(EntityTypeBuilder<LocationPayments> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(s => s.Amount).HasColumnType("decimal(18,2)");
            builder.Property(e => e.Status).HasConversion<int>();
            builder.Property(e => e.Type).HasConversion<int>();
            builder.Property(s => s.Note).HasMaxLength(500);
            builder.Property(c => c.PaymentDate).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.Location)
                .WithMany(x => x.LocationPayments)
                .HasForeignKey(x => x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Receiver)
               .WithMany(x => x.LocationPaymentsReceiver)
               .HasForeignKey(x => x.ReceiverId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
