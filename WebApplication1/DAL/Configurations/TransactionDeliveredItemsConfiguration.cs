using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionDeliveredItemsConfiguration : IEntityTypeConfiguration<TransactionItemDelivered>
    {
        public void Configure(EntityTypeBuilder<TransactionItemDelivered> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");


            builder.HasOne(x => x.TransactionItem)
                .WithMany(x => x.TransactionItemsDelivered)
                .HasForeignKey(x => x.TransactionItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
