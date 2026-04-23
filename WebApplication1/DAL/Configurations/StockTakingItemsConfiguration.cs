using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockTakingItemsConfiguration : IEntityTypeConfiguration<StockTakingItem>
    {
        public void Configure(EntityTypeBuilder<StockTakingItem> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");


            builder.HasOne(x=>x.Item)
                .WithMany(x=> x.StockTakingItems)
                .HasForeignKey(x=> x.ItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.StockTaking)
                .WithMany(x => x.StockTakingItems)
                .HasForeignKey(x => x.StockTakingId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
