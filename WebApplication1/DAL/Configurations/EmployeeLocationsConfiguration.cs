using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class EmployeeLocationsConfiguration : IEntityTypeConfiguration<EmployeeLocation>
    {
        public void Configure(EntityTypeBuilder<EmployeeLocation> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x=>x.Employee)
                .WithMany(x=> x.EmployeeLocations)
                .HasForeignKey(x=> x.EmployeeId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Location)
               .WithMany(x => x.EmployeeLocations)
               .HasForeignKey(x => x.LocationId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.EmployeeId);
            builder.HasIndex(x => x.LocationId);
            builder.HasIndex(x => new {x.EmployeeId, x.LocationId});
        }
    }
}
