using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
           // builder.ToTable("AuditLogs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(50);

            // ✅ CHANGE: Use "text" for PostgreSQL instead of "nvarchar(max)"
            builder.Property(e => e.OldValue)
                .HasColumnType("text");

            builder.Property(e => e.NewValue)
                .HasColumnType("text");

            builder.Property(e => e.ChangedBy)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ChangedByName)
                .HasMaxLength(100);

            // ✅ CHANGE: Use "timestamp with time zone" or just "timestamp"
            builder.Property(e => e.ChangedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.IpAddress)
                .HasMaxLength(45);

            builder.Property(e => e.UserAgent)
                .HasMaxLength(500);

            builder.Property(e => e.RequestPath)
                .HasMaxLength(500);

            builder.Property(e => e.RequestMethod)
                .HasMaxLength(10);

            builder.Property(e => e.Reason)
                .HasMaxLength(500);

            // ✅ JSONB is correct for PostgreSQL
            builder.Property(e => e.Metadata)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                );

            // ✅ JSONB is correct for PostgreSQL
            builder.Property(e => e.Details)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    v => JsonSerializer.Deserialize<List<AuditDetail>>(v, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                );

            // Indexes
            builder.HasIndex(e => new { e.EntityType, e.EntityId })
                .HasDatabaseName("IX_AuditLogs_EntityType_EntityId");

            builder.HasIndex(e => e.ChangedAt)
                .HasDatabaseName("IX_AuditLogs_ChangedAt");

            builder.HasIndex(e => e.Action)
                .HasDatabaseName("IX_AuditLogs_Action");

            builder.HasIndex(e => e.ChangedBy)
                .HasDatabaseName("IX_AuditLogs_ChangedBy");
        }
    }
}