using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {

            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x=>x.Customer)
                .WithMany(x=> x.Sales)
                .HasForeignKey(x=> x.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.SalesPerson)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.SalesPersonId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Transaction)
                .WithOne()
                .HasForeignKey<Sale>(x => x.TransactionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
               .WithMany(x => x.Sales)
               .HasForeignKey(x => x.LocationId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
