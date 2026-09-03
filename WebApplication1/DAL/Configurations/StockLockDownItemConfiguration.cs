using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockLockDownItemConfiguration : IEntityTypeConfiguration<StockLockDownItem>
    {
        public void Configure(EntityTypeBuilder<StockLockDownItem> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ItemId);

            builder.HasIndex(x => x.StockLockDownRequestId);

            builder.Property(x => x.ItemId).IsRequired();

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.StockLockDownRequest)
                .WithMany(x => x.StockLockDownItems)
                .HasForeignKey(x => x.StockLockDownRequestId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Item)
                .WithMany(x => x.StockLockDownItems)
                .HasForeignKey(x=> x.ItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}