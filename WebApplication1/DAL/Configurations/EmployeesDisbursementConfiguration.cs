using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class EmployeesDisbursementConfiguration : IEntityTypeConfiguration<EmployeeDisbursement>
    {
        public void Configure(EntityTypeBuilder<EmployeeDisbursement> builder)
        {
            // Apply base PostgreSQL configuration
            BaseEntityConfiguration.ConfigureForPostgres(builder);


            builder.HasOne(x => x.Location)
                .WithMany(x => x.EmployeeDisbursements)
                .HasForeignKey(x => x.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Employee)
               .WithMany(x => x.EmployeesDisbursements)
               .HasForeignKey(x => x.EmployeeId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x=> x.Transaction)
                .WithOne()
                .HasForeignKey<EmployeeDisbursement>(x=>x.TransactionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
