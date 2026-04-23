using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionPaymentsConfiguration : IEntityTypeConfiguration<TransactionPayment>
    {
        public void Configure(EntityTypeBuilder<TransactionPayment> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);

            builder.Property(s => s.PaymentMethod).HasConversion<int>();
            builder.Property(s => s.Amount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.PaymentDate).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(e => e.PaymentMethod).HasConversion<int>();

            builder.HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionPayments)
                .HasForeignKey(x => x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
