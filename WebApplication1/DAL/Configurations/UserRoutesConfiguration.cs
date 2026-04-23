using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class UserRoutesConfiguration : IEntityTypeConfiguration<UserRoutes>
    {
        public void Configure(EntityTypeBuilder<UserRoutes> builder)
        {

            // Configure REQUIRED relationship with Company
            builder.HasOne(u => u.PositionRoutes)
                  .WithMany(c => c.UserRoutes)
                  .HasForeignKey(u => u.PositionRouteId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

            builder.HasOne(u => u.User)
                 .WithMany(c => c.UserRoutes)
                 .HasForeignKey(u => u.UserId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);

        }
    }
}