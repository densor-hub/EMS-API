using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class ItemLocationsConfiguration : IEntityTypeConfiguration<ItemLocation>
    {
        public void Configure(EntityTypeBuilder<ItemLocation> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x=>x.Item)
                .WithMany(x=> x.ItemLocations)
                .HasForeignKey(x=> x.ItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Location)
               .WithMany(x => x.ItemLocations)
               .HasForeignKey(x => x.LocationId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ItemId);
            builder.HasIndex(x => x.LocationId);
            builder.HasIndex(x => new {x.ItemId, x.LocationId});
        }
    }
}
