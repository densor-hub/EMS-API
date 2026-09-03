using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class StockTakeItemSubmissionConfiguration : IEntityTypeConfiguration<StockTakeItemSubmission>
    {
        public void Configure(EntityTypeBuilder<StockTakeItemSubmission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedById).IsRequired().HasMaxLength(38);
            builder.Property(x => x.VerifiedById).IsRequired(false).HasMaxLength(38);
            builder.Property(x => x.SystemAvailableQuantity).IsRequired();
            builder.Property(x => x.PhysicalUnitOfMeasureQuantity).IsRequired();
            builder.Property(x => x.PhysicalAdditionalPiecesQuantity).IsRequired();

            builder.Property(c => c.ApprovedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(c => c.CreatedAt).IsRequired(true).HasColumnType("timestamp with time zone");



            builder.HasOne(x => x.StockLockDownItem)
                   .WithMany(x => x.Submissions)
                   .HasForeignKey(x => x.StockLockDownItemId)
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedByUser)
                   .WithMany(x => x.StockTakeItemSubmissions)
                   .HasForeignKey(x => x.CreatedById)
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.VerifiedByByUser)
                   .WithMany(x => x.StockTakeItemVerifications)
                   .HasForeignKey(x => x.VerifiedById)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}