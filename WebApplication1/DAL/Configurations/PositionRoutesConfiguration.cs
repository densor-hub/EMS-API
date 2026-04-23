using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class PositionRoutesConfiguration : IEntityTypeConfiguration<PositionRoutes>
    {
        public void Configure(EntityTypeBuilder<PositionRoutes> builder)
        {

            // Configure REQUIRED relationship with Company
            builder.HasOne(u => u.Position)
                  .WithMany(c => c.PositionRoutes)
                  .HasForeignKey(u => u.PositionId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

            builder.HasOne(u => u.ApplicationRoutes)
                 .WithMany(c => c.PositionRoutes)
                 .HasForeignKey(u => u.AppRouteId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);

        }
    }
}