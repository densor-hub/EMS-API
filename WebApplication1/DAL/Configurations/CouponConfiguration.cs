using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.Code).IsRequired().HasMaxLength(15);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(c => c.Used).IsRequired().HasDefaultValue(false);


            builder.HasIndex(x => x.LocationId);
            //builder.HasIndex(x => x.TransactionId);

            //builder.HasOne(x => x.Transaction)
            //    .WithOne()
            //    .HasForeignKey<Transaction>(x => x.CouponId)
            //    .IsRequired(false);

            builder.HasOne(x => x.Location)
                .WithMany(x=> x.Coupons)
                .HasForeignKey(x=> x.LocationId)
                .IsRequired(false);


        }
    }
}
