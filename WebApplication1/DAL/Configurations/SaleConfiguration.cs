using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            // ⚠️ Remove UseIdentityColumn — the app assigns IncrementalId
            builder.Property(x => x.IncrementalId).ValueGeneratedNever();

            // Postgres "date" column for the sale day
            builder.Property(x => x.SaleDate).IsRequired().HasColumnType("date");

            // ✅ Per-location, per-day uniqueness
            builder.HasIndex(x => new { x.LocationId, x.SaleDate, x.IncrementalId })
                .IsUnique()
                .HasDatabaseName("IX_Sales_Location_Date_Incremental");

            // Existing FKs...
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.SalesPerson)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.SalesPersonId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Transaction)
                .WithOne()
                .HasForeignKey<Sale>(x => x.TransactionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Location>()
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
