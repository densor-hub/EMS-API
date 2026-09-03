using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class DailyTransactionCounterConfiguration : IEntityTypeConfiguration<DailyTransactionCounter>
    {
        public void Configure(EntityTypeBuilder<DailyTransactionCounter> builder)
        {

            // Configure REQUIRED relationship with Company
            builder.HasKey(x=> x.Id);
            builder.HasIndex(x => x.LocationId);
            builder.HasIndex(x => x.CounterDate);


        }
    }
}