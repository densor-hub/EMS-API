using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionDeliveriesConfiguration : IEntityTypeConfiguration<TransactionDelivery>
    {
        public void Configure(EntityTypeBuilder<TransactionDelivery> builder)
        {
            // Apply base PostgreSQL configuration
            //BaseEntityConfiguration.ConfigureForPostgres(builder);

            //builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            //builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            //builder.HasOne(x=>x.Transaction)
            //    .WithMany(x=> x.TransactionDeliveries)
            //    .HasForeignKey(x=> x.TransactionId)
            //    .IsRequired(true)
            //    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
