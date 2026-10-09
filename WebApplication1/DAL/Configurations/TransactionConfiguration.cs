using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(e => e.TransactionResultsType).HasConversion<int>();

            //builder.Property(s => s.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(s => s.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(s => s.Notes).HasMaxLength(500);

            builder.HasIndex(x => x.CouponId);
            //builder.HasIndex(x => x.TransactionType);
            builder.HasIndex(x => x.TransactionType);

            builder.HasOne(x => x.Coupon)
                .WithOne()
                .HasForeignKey<Transaction>(x => x.CouponId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x=> x.Location)
                .WithMany(x=> x.Transactions)
                .HasForeignKey(x=> x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
