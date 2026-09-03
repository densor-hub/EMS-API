using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class VehicleAssignmentConfiguration : IEntityTypeConfiguration<VehicleAssignments>
    {
        public void Configure(EntityTypeBuilder<VehicleAssignments> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.DriverId);
            builder.HasIndex(x => x.GeneralStatus);

            builder.HasOne(x => x.Vehicle)
                .WithMany(x=> x.VehicleAssignments)
                .HasForeignKey(x=> x.VehicleId)
                .IsRequired(false);


            builder.HasOne(x => x.Driver)
                .WithMany(x => x.VehicleAssignments)
                .HasForeignKey(x => x.DriverId)
                .IsRequired(false);


        }
    }
}
