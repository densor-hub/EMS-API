using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.DAL.Configurations
{
    public static class BaseEntityConfiguration
    {
        public static void ConfigureForPostgres<T>(EntityTypeBuilder<T> builder) where T : class
        {
            // Postgres convention: lowercase table names (only if your tables were
            // created lowercase — otherwise skip this).
            var entityName = builder.Metadata.GetTableName();
            if (!string.IsNullOrEmpty(entityName))
            {
              //  builder.ToTable(entityName.ToLowerInvariant());
            }

            foreach (var property in builder.Metadata.GetProperties())
            {
                // ── GUIDs → uuid ─────────────────────────────────────────────
                if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
                {
                    property.SetColumnType("uuid");
                    continue;
                }

                // ── string Id / *Id → text ───────────────────────────────────
                if (property.ClrType == typeof(string) &&
                    (property.Name == "Id" || property.Name.EndsWith("Id")))
                {
                    property.SetColumnType("text");
                    continue;
                }

                // ── CreatedAt → timestamptz NOT NULL DEFAULT now() ───────────
                if (property.Name == "CreatedAt" &&
                    (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)))
                {
                    // ✅ Non-generic overload — uses the existing CLR type
                    var created = builder.Property(property.Name)
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()")
                        .ValueGeneratedOnAdd();

                    // Only mark NOT NULL if the CLR property is DateTime (non-nullable).
                    if (property.ClrType == typeof(DateTime))
                        created.IsRequired();

                    continue;
                }

                // ── UpdatedAt → timestamptz NULL ─────────────────────────────
                if (property.Name == "UpdatedAt" &&
                    (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)))
                {
                    builder.Property(property.Name)
                        .HasColumnType("timestamp with time zone");

                    continue;
                }
            }
        }
    }
}