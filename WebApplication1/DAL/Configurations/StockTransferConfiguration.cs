using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
    {
        public void Configure(EntityTypeBuilder<StockTransfer> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(e => e.Status).HasConversion<int>();

            builder.HasOne(x=>x.InitiatedBy)
                .WithMany(x=> x.InitiatedStockTransfers)
                .HasForeignKey(x=> x.InitiatedById)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ApprovedBy)
                .WithMany(x => x.ApprovedStockTransfers)
                .HasForeignKey(x => x.ApprovedByUser)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FromLocation)
               .WithMany(x => x.StockTransfersFrom)
               .HasForeignKey(x => x.FromLocationId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToLocation)
               .WithMany(x => x.StockTransfersTo)
               .HasForeignKey(x => x.ToLocationId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
