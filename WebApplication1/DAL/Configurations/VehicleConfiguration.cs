using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.Code).IsRequired().HasMaxLength(10);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasIndex(x => x.LocationId);

            builder.HasOne(x => x.Location)
                .WithMany(x=> x.Vehicles)
                .HasForeignKey(x=> x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
