using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class LocationManagementConfiguration : IEntityTypeConfiguration<LocationManangement>
    {
        public void Configure(EntityTypeBuilder<LocationManangement> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(c => c.Status).IsRequired();

            builder.HasOne(x=> x.Location)
                .WithMany(s=> s.LocationManangements)
                .HasForeignKey(sm => sm.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Manager)
                .WithMany(s => s.ShopManangement)
                .HasForeignKey(sm => sm.ManagerId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
