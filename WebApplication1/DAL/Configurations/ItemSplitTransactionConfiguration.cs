using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class ItemSplitTransactionConfiguration
        : IEntityTypeConfiguration<ItemSplitTransaction>
    {
        public void Configure(EntityTypeBuilder<ItemSplitTransaction> builder)
        {
            builder.ToTable("ItemSplitTransactions");

            // ---- Primary key ----
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            // ---- Audit ----
            builder.Property(x => x.CreatedById)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            // ---- Snapshots ----
            builder.Property(x => x.SourceItemNameSnapShot)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.TragetItemNameSnapShot)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.TargetItemQuantitySnapShot)
                   .IsRequired();

            builder.Property(x => x.LocationId)
                   .IsRequired();

            // ---- Relationships ----
            builder.HasOne(x => x.ItemSplitSetup)
                   .WithMany(s => s.ItemSplitTransactions)
                   .HasForeignKey(x => x.ItemSplitSetUpId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ✅ FIX: use the inverse collection on Location
            builder.HasOne(x => x.Location)
                   .WithMany(l => l.ItemSplitTransactions)
                   .HasForeignKey(x => x.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- Indexes ----
            builder.HasIndex(x => new { x.ItemSplitSetUpId, x.CreatedAt })
                   .HasDatabaseName("IX_ItemSplitTransaction_Setup_CreatedAt");

            builder.HasIndex(x => new { x.LocationId, x.CreatedAt })
                   .HasDatabaseName("IX_ItemSplitTransaction_Location_CreatedAt");

            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}