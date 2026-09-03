using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockLockDownRequestConfiguration : IEntityTypeConfiguration<StockLockDownRequest>
    {
        public void Configure(EntityTypeBuilder<StockLockDownRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.TransactionDate);

            builder.Property(x => x.TransactionNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CreatedById).IsRequired().HasMaxLength(50);


            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.Location)
                .WithMany(x => x.StockLockDownRequests)
                .HasForeignKey(x => x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedByUser)
              .WithMany(x => x.StockLockDownRequests)
              .HasForeignKey(x => x.CreatedById)
              .IsRequired(true)
              .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
