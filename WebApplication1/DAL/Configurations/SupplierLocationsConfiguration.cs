using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SupplierLocationsConfiguration : IEntityTypeConfiguration<SupplierLocation>
    {
        public void Configure(EntityTypeBuilder<SupplierLocation> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x=>x.Location)
                .WithMany(x=> x.SupplierLocations)
                .HasForeignKey(x=> x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Supplier)
               .WithMany(x => x.SupplierLocations)
               .HasForeignKey(x => x.SupplierId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
