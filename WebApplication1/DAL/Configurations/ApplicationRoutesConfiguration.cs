using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class ApplicationRoutesConfiguration : IEntityTypeConfiguration<ApplicationRoutes>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoutes> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Custom configurations for ApplicationUser
           // builder.HasIndex(u => u.ParentId).IsUnique();


            builder.Property(u => u.Path).HasMaxLength(300);
            builder.Property(u => u.Status)
                .IsRequired()
                .HasDefaultValue(true);

            // Configure REQUIRED relationship with Company
            builder.HasOne(u => u.Parent)
                  .WithMany(c => c.Children)
                  .HasForeignKey(u => u.ParentId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

        }
    }
}