using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(e => e.UnitOfMeasure).HasConversion<int>(); // This will store enum as int
            builder.Property(e => e.Category).HasConversion<int>();
            builder.HasOne(x=> x.Company)
                .WithMany(x=> x.Items)
                .HasForeignKey(x=>x.CompanyId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.SellingPrice).IsRequired();
            builder.Property(c => c.CostPrice).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasIndex(x => x.Category);
           
        }
    }
}
