using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockTakingConfiguration : IEntityTypeConfiguration<StockTaking>
    {
        public void Configure(EntityTypeBuilder<StockTaking> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(s => s.Notes).HasMaxLength(500);
            builder.Property(c => c.StockTakingDate).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(c => c.NextStockTakingDate).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(e => e.Stage).HasConversion<int>();

            builder.HasOne(x=>x.Location)
                .WithMany(x=> x.StockTakings)
                .HasForeignKey(x=> x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Conductor)
                .WithMany(x => x.ConductedStockTakings)
                .HasForeignKey(x => x.ConductedBy)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Verifier)
                .WithMany(x => x.VerifiedStockTakings)
                .HasForeignKey(x => x.VerifiedBy)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
