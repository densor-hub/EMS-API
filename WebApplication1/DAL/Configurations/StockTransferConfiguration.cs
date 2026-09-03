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

            builder.Property(c => c.CreatedAt).IsRequired(true).HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(e => e.Status).HasConversion<int>();

            builder.HasIndex(x => x.TransactionId);
            builder.HasIndex(x => x.RequesterId);
            builder.HasIndex(x => x.ResponderId);

            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne(x=>x.Transaction)
                .WithOne()
                .HasForeignKey<StockTransfer>(x=> x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Requester)
                .WithMany(x => x.StockTransferRequests)
                .HasForeignKey(x=> x.RequesterId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Responder)
               .WithMany(x => x.StockTransferResponds)
               .HasForeignKey(x => x.ResponderId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
