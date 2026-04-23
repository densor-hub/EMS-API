using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Apply base PostgreSQL configuration
            // BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Custom configurations for ApplicationUser
            builder.ToTable("AspNetUsers");
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt).IsRequired(false);
            builder.Property(u => u.Status)
                .IsRequired()
                .HasDefaultValue(true);

            // Configure REQUIRED relationship with Company
            builder.HasOne(u => u.Company)
                  .WithMany(c => c.Users)
                  .HasForeignKey(u => u.CompanyId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);

            // Add index on CompanyId for better query performance
            builder.HasIndex(u => u.CompanyId);

            // Configure Identity-specific properties if needed
            builder.Property(u => u.UserName)
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedUserName)
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                .HasMaxLength(256);
        }
    }
}