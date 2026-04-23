using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.DAL.Configurations
{
    public  static class BaseEntityConfiguration
    {
        public static void ConfigureForPostgres<T>(EntityTypeBuilder<T> builder) where T : class
        {
            // Get the entity type name
            var entityName = builder.Metadata.GetTableName();

            // Convert table name to lowercase for PostgreSQL
            builder.ToTable(entityName);

            // Configure GUID properties as uuid
            foreach (var property in builder.Metadata.GetProperties())
            {
                if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
                {
                    property.SetColumnType("uuid");
                }

                // Configure string IDs as text
                if (property.ClrType == typeof(string) &&
                    (property.Name == "Id" || property.Name.EndsWith("Id")))
                {
                    property.SetColumnType("text");
                }
            }
        }
    }
}
