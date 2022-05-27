using FarmMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmMonitor.Infrastructure.Database.EntityTypeConfigurations
{
    internal class TelemetryEntityTypeConfiguration : IEntityTypeConfiguration<TelemetryEntity>
    {
        public void Configure(EntityTypeBuilder<TelemetryEntity> builder)
        {
            builder.ToTable(nameof(TelemetryEntity));

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId);
        }
    }
}
