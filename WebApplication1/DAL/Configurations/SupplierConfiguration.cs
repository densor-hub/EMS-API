using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(200);
            builder.HasIndex(c => c.Email).IsUnique();
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Phone).HasMaxLength(20);
            builder.Property(c => c.Address).HasMaxLength(500).IsRequired(false);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x=> x.Company)
                .WithMany(x=> x.Suppliers)
                .HasForeignKey(x=> x.CompanyId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(x => x.Location)
            //    .WithMany(x => x.Suppliers)
            //    .HasForeignKey(x => x.LocationId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
