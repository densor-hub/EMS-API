using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.DAL.Configurations
{
    public class ItemSplitSetupConfiguration : IEntityTypeConfiguration<ItemSplitSetup>
    {
        public void Configure(EntityTypeBuilder<ItemSplitSetup> builder)
        {
            builder.ToTable("ItemSplitSetups");

            // ---- Primary key ----
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            // ---- Quantities ----
            builder.Property(x => x.SourceQuantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(x => x.TargetQuantity)
                   .IsRequired();

            // ---- Audit ----
            builder.Property(x => x.CreatedById)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.UpdatedById)
                   .HasMaxLength(450);

            // ---- Status ----
            builder.Property(x => x.GeneralStatus)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasDefaultValue(GeneralStatus.Active);

            // ---- Relationships ----
            builder.HasOne(x => x.SourceItem)
                   .WithMany(i => i.SourceItem)
                   .HasForeignKey(x => x.SourceItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TargetItem)
                   .WithMany(i => i.TargetItem)
                   .HasForeignKey(x => x.TargetItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.UpdatedById)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            // ---- Indexes ----
            builder.HasIndex(x => x.SourceItemId);
            builder.HasIndex(x => x.TargetItemId);
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.UpdatedById);
            builder.HasIndex(x => x.GeneralStatus);

            builder.HasIndex(x => new { x.SourceItemId, x.GeneralStatus })
                   .HasDatabaseName("IX_ItemSplitSetup_SourceItem_Status");

            builder.HasIndex(x => new { x.SourceItemId, x.TargetItemId })
                   .IsUnique()
                   .HasDatabaseName("UX_ItemSplitSetup_Source_Target");
        }
    }
}