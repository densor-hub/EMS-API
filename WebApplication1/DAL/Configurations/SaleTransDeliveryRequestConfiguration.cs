using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SaleTransDeliveryRequestConfiguration : IEntityTypeConfiguration<SaleTransDeliveryRequest>
    {
        public void Configure(EntityTypeBuilder<SaleTransDeliveryRequest> builder)
        {

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.Sale)
                .WithMany(x => x.SaleTransDeliveryRequests)
                .HasForeignKey(x => x.SaleId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany(x => x.TransactionDeliveryRequests)
                .HasForeignKey(x => x.CreatedById)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany(x => x.TransactionDeliveryRequestUpdates)
                .HasForeignKey(x => x.UpdatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
