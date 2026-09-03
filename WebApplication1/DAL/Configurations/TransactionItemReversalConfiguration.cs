using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionItemReversalConfiguration : IEntityTypeConfiguration<TransactionItemReversal>
    {
        public void Configure(EntityTypeBuilder<TransactionItemReversal> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Custom configurations for ApplicationUser
             builder.HasIndex(u => u.Id).IsUnique();


            builder.HasIndex(u => u.TransactionItemDeliveredId);

            builder.HasOne(u => u.TransactionItemDelivered)
                  .WithMany(c => c.TransactionItemReversals)
                  .HasForeignKey(u => u.TransactionItemDeliveredId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(true);

        }
    }
}