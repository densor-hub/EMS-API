using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Configurations
{
    public class SystemControlConfiguration : IEntityTypeConfiguration<SystemControl>
    {
        public void Configure(EntityTypeBuilder<SystemControl> builder)
        {
             builder.HasOne(x => x.Location)
                .WithOne()
                .HasForeignKey<SystemControl>(x => x.LocationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}