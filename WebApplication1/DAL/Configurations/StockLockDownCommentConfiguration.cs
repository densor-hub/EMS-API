using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockLockDownCommentConfiguration : IEntityTypeConfiguration<StockLockDownComment>
    {
        public void Configure(EntityTypeBuilder<StockLockDownComment> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.StockLockDownRequestId);
            builder.HasIndex(x => x.TransactionType);

            // Properties configuration
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.StockLockDownRequest)
                .WithMany(x=> x.StockLockDownComments)
                .HasForeignKey(x => x.StockLockDownRequestId)
                .IsRequired(true);

            builder.HasOne(x => x.CreatedBy)
               .WithMany(x => x.StockLockDownComments)
               .HasForeignKey(x => x.CreatedById)
               .IsRequired(true);



        }
    }
}
