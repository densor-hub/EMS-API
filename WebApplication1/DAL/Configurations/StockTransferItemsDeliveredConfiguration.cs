using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockTransferItemsDeliveredConfiguration : IEntityTypeConfiguration<StockTransferItemsDelivered>
    {
        public void Configure(EntityTypeBuilder<StockTransferItemsDelivered> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");


            builder.HasOne(x => x.StockTransferItem)
                .WithMany(x => x.stockTransferItemsDelivered)
                .HasForeignKey(x => x.StockTransferItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
