using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class FinancialServiceProviderContactPersonConfiguration : IEntityTypeConfiguration<FinancialServiceProviderContactPerson>
    {
        public void Configure(EntityTypeBuilder<FinancialServiceProviderContactPerson> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.FullName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(10);
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(c => c.UpdatedAt).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(x => x.IncrementalId).UseIdentityColumn();


            builder.HasIndex(x => x.FinancialServiceProviderId);
            builder.HasIndex(x => x.Email);
            builder.HasIndex(x => x.Code);

            builder.HasOne(x => x.FinancialServiceProvider)
                .WithMany(x=> x.ContactPersons)
                .HasForeignKey(x=> x.FinancialServiceProviderId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);





        }
    }
}
