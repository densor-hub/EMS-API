using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
    {
        public void Configure(EntityTypeBuilder<StockLevel> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.HasKey(x => x.Id);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasIndex(x => new { x.ItemId, x.LocationId }).IsUnique();

            //builder.Property(s => s.ActualQuantity).HasColumnType("decimal(18,2)");
            //builder.Property(s => s.AvailableQuanity).HasColumnType("decimal(18,2)");

            builder.HasOne(x=>x.Location)
                .WithMany(x=> x.StockLevels)
                .HasForeignKey(x=> x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Item)
                .WithMany(x => x.StockLevel)
                .HasForeignKey(x => x.ItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
