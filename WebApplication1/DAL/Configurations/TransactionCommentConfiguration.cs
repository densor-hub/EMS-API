using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class TransactionCommentConfiguration : IEntityTypeConfiguration<TransactionComment>
    {
        public void Configure(EntityTypeBuilder<TransactionComment> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.TransactionId);
            builder.HasIndex(x => x.TransactionType);

            // Properties configuration
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.Transaction)
                .WithMany(x=> x.CommentsAndLog)
                .HasForeignKey(x => x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x=> x.CreatedByUser)
                .WithMany(x=> x.TransactionComments)
                .HasForeignKey(x=> x.CreatedById)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);


          

        }
    }
}
