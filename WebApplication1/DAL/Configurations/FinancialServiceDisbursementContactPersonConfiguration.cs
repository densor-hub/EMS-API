using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class FinancialServiceDisbursementContactPersonConfiguration : IEntityTypeConfiguration<FinancialServiceDisbursementContactPerson>
    {
        public void Configure(EntityTypeBuilder<FinancialServiceDisbursementContactPerson> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);
            // Primary Key
            builder.HasKey(c => c.Id);


            //builder.HasIndex(x => x.DisbursementId);
            builder.HasIndex(x => x.DisbursementId);
            builder.HasIndex(x => x.ContactPersonId);

            builder.HasOne(x => x.ContactPerson)
                .WithMany(x=> x.Disbursements)
                .HasForeignKey(x => x.ContactPersonId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Disbursement)
               .WithMany(x => x.FinancialServiceDisbursementContactPersons)
               .HasForeignKey(x => x.DisbursementId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
