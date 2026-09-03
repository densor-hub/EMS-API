using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {


            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Code).IsUnique();
            builder.Property(u => u.Name).HasMaxLength(100);

        }
    }
}