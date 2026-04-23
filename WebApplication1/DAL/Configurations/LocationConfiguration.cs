using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.HasIndex(c => c.Email).IsUnique();
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Address) .HasMaxLength(500).IsRequired(false);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(c => c.Status).IsRequired();
            builder.Property(e => e.TypeOfLocation).HasConversion<int>();


            builder.HasOne(x=> x.Company)
                .WithMany(c=> c.Locations)
                .HasForeignKey(s => s.CompanyId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.TypeOfLocation);
            builder.HasIndex(x => new { x.CompanyId, x.TypeOfLocation });


        }
    }
}
