using FarmManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmManager.Infrastructure.Database.EntityTypeConfigurations
{
    internal class DeviceEntityTypeConfiguration : IEntityTypeConfiguration<DeviceEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceEntity> builder)
        {
            builder.ToTable(nameof(DeviceEntity));

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Farm)
                .WithMany()
                .HasForeignKey(e => e.FarmId);

        }
    }
}
