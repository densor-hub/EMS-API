using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockTransferItemsConfiguration : IEntityTypeConfiguration<StockTransferItem>
    {
        public void Configure(EntityTypeBuilder<StockTransferItem> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x=>x.Item)
                .WithMany(x=> x.StockTransferItems)
                .HasForeignKey(x=> x.ItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.StockTransfer)
                .WithMany(x => x.StockTransferItems)
                .HasForeignKey(x => x.StockTransferId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
