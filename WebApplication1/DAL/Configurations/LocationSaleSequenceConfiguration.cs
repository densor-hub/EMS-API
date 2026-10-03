using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class LocationSaleSequenceConfiguration : IEntityTypeConfiguration<LocationSaleSequence>
    {
        public void Configure(EntityTypeBuilder<LocationSaleSequence> builder)
        {
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(x => x.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.Property(x => x.LocationId).IsRequired();
            builder.Property(x => x.LastSaleIncremental).IsRequired().HasDefaultValue(0);

            builder.HasIndex(x => x.LocationId).IsUnique();

            builder.HasOne<Location>()
                .WithOne()
                .HasForeignKey<LocationSaleSequence>(x => x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}