using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            // Primary Key
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.TransactionId);
            builder.HasIndex(c => c.CurrencyId);

            // Properties configuration
            //builder.Property(c => c.Amount).IsRequired().HasVa(200);
            //builder.Property(c => c.Balance).IsRequired().HasVa(200);
            builder.Property(e => e.PaymentStatus).HasConversion<int>();
            builder.Property(e => e.PaymentMethod).HasConversion<int>();

            builder.Property(c => c.PaymentDate).IsRequired().HasColumnType("timestamp with time zone");

            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionPayments)
                .HasForeignKey(x => x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.CurrencyId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Coupon)
               .WithOne()
               .HasForeignKey<Payment>(x => x.CouponId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ConfirmationToken)
               .WithOne()
               .HasForeignKey<Payment>(x => x.ConfirmationTokenId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
