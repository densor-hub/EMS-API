using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.DAL.Configurations
{
    public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
    {
        public void Configure(EntityTypeBuilder<StockAdjustment> builder)
        {
            builder.ToTable("StockAdjustments");

            // ---- Primary key ----
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedNever();      // domain assigns Guid in Create()

       //      // ---- Quantity ----
       //      builder.Property(x => x.Quantity)
       //             .IsRequired();

            // ---- Dates ----
       //      builder.Property(x => x.CreatedAt)
       //             .IsRequired();

       //      builder.Property(x => x.TransactionDate)
       //             .IsRequired();

            // ---- Status ----
       //      builder.Property(x => x.GeneralStatus)
       //             .IsRequired()
       //             .HasConversion<int>()
       //             .HasDefaultValue(GeneralStatus.Active);

            // ---- Audit ----
       //      builder.Property(x => x.CreatedById)
       //             .IsRequired()
       //             .HasMaxLength(450);

            builder.Property(x => x.ApprovedById)
                   .HasMaxLength(450);

            // ---- Relationships ----
            builder.HasOne(x => x.Transaction)
                .WithOne()
                .HasForeignKey<StockAdjustment>(x => x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

          

            builder.HasOne(x => x.Supplier)
                   .WithMany(x=> x.StockAdjustments)
                   .HasForeignKey(x => x.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(x => x.ApprovedBy)
                   .WithMany()
                   .HasForeignKey(x => x.ApprovedById)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            // ---- Indexes ----
            builder.HasIndex(x => x.TransactionId);
            builder.HasIndex(x => x.SupplierId);
       //      builder.HasIndex(x => x.C);
            builder.HasIndex(x => x.ApprovedById);
       //      builder.HasIndex(x => x.GeneralStatus);
       //      builder.HasIndex(x => x.TransactionDate);

            // Hot query: adjustments for a given item at a given location,
            // in transaction-date order — the classic "stock ledger" view.
            builder.HasIndex(x => new { x.TransactionId, x.SupplierId, x.ApprovedById });
                 //  .HasDatabaseName("IX_StockAdjustment_Item_Location_TransactionDate");
        }
    }
}