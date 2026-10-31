using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SaleTransDeliveryRequestConfiguration : IEntityTypeConfiguration<SaleTransDeliveryRequest>
    {
        public void Configure(EntityTypeBuilder<SaleTransDeliveryRequest> builder)
        {

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

  // ⚠️ Remove UseIdentityColumn — the app assigns IncrementalId
            builder.Property(x => x.IncrementalId).ValueGeneratedNever();

            // Postgres "date" column for the sale day
            builder.Property(x => x.DateOnly).IsRequired().HasColumnType("date");

            // ✅ Per-location, per-day uniqueness
            builder.HasIndex(x => new { x.LocationId, x.DateOnly, x.IncrementalId })
                .IsUnique();
              //  .HasDatabaseName("IX_Sales_Location_Date_Incremental");

            builder.HasOne(x => x.Sale)
                .WithMany(x => x.SaleTransDeliveryRequests)
                .HasForeignKey(x => x.SaleId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany(x => x.SaleTransDeliveryRequests)
                .HasForeignKey(x => x.LocationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany(x => x.TransactionDeliveryRequests)
                .HasForeignKey(x => x.CreatedById)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany(x => x.TransactionDeliveryRequestUpdates)
                .HasForeignKey(x => x.UpdatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
