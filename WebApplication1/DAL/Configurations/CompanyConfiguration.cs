using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.HasIndex(c => c.Email).IsUnique();
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(c => c.PhoneNumber).IsUnique();
            builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Address).HasMaxLength(500).IsRequired(false);
            builder.Property(c => c.Website) .HasMaxLength(100).IsRequired(false);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(c => c.Status).IsRequired().HasDefaultValue(true);



        }
    }
}
