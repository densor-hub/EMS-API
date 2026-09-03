using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class DisbursementTokensConfiguration : IEntityTypeConfiguration<PaymentConfirmationToken>
    {
        public void Configure(EntityTypeBuilder<PaymentConfirmationToken> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
           // builder.HasKey(c => c.Id);
           builder.HasKey(t => t.Id);

            // Properties configuration
            builder.Property(c => c.Token).IsRequired().HasMaxLength(200);
            builder.Property(c => c.UserEmail).IsRequired(false).HasMaxLength(200);
            builder.Property(c => c.IpAddress).IsRequired(false).HasMaxLength(50);
            builder.Property(c => c.UserAgent).IsRequired(false).HasMaxLength(500);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");


            builder.HasIndex(e => e.Token).IsUnique();
            builder.HasIndex(e => e.PaymentId);
            builder.HasIndex(e => e.ExpiresAt);
            builder.HasIndex(e => new { e.Token, e.IsUsed });

           
            builder.HasOne(e => e.Payment)
            .WithOne(x=> x.ConfirmationToken)
            .HasForeignKey<Payment>(e => e.ConfirmationTokenId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
