using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SupplierItemCostPriceConfiguration : IEntityTypeConfiguration<SupplierItemCostPrice>
    {
        public void Configure(EntityTypeBuilder<SupplierItemCostPrice> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Primary Key
            builder.HasKey(c => c.Id);

            builder.HasIndex(x => x.SupplierId);
            builder.HasIndex(x => x.ItemId);
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne(x=> x.Supplier)
                .WithMany(x=> x.SupplierItemCostPrices)
                .HasForeignKey(x=> x.SupplierId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Item)
                .WithMany(x => x.SupplierItemCostPrices)
                .HasForeignKey(x => x.ItemId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ApplicationUser)
               .WithMany(x => x.SupplierItemCostPrice)
               .HasForeignKey(x => x.CreatedById)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
