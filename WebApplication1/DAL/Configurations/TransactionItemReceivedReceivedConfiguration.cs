using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionItemReceivedReceivedConfiguration : IEntityTypeConfiguration<TransactionItemReceived>
    {
        public void Configure(EntityTypeBuilder<TransactionItemReceived> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.TransactionItemId);


            // Properties configuration
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.TransactionItem)
                .WithMany(x => x.TransactionItemReceived)
                .HasForeignKey(x => x.TransactionItemId)
                .IsRequired(true);



        }
    }
}
