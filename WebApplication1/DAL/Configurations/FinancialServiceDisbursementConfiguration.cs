using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class FinancialServiceDisbursementConfiguration : IEntityTypeConfiguration<FinancialServiceDisbursement>
    {
        public void Configure(EntityTypeBuilder<FinancialServiceDisbursement> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);


            //builder.HasIndex(x => x.DisbursementId);
            builder.HasIndex(x => x.FinancialServiceProviderId);

            builder.HasOne(x => x.Transaction)
                .WithMany()
                .HasForeignKey(x=> x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FinancialServiceProvider)
               .WithMany(x=> x.FinancialServiceDisbursements)
               .HasForeignKey(x => x.FinancialServiceProviderId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.ContactPerson)
               .WithMany(x => x.FinancialServiceDisbursements)
               .HasForeignKey(x => x.ContactPersonId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
