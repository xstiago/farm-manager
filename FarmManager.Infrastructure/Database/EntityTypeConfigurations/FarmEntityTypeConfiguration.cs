using FarmManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmManager.Infrastructure.Database.EntityTypeConfigurations
{
    internal class FarmEntityTypeConfiguration : IEntityTypeConfiguration<FarmEntity>
    {
        public void Configure(EntityTypeBuilder<FarmEntity> builder)
        {
            builder.ToTable(nameof(FarmEntity));

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired(true);
        }
    }
}
