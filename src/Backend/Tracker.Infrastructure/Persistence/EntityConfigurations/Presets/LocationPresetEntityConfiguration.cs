using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Presets;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Presets;

public sealed class LocationPresetEntityConfiguration : IEntityTypeConfiguration<LocationPreset>
{
    public void Configure(EntityTypeBuilder<LocationPreset> builder)
    {
        builder.ToTable("location_preset");

        builder.ConfigureAuditable();

        builder.Property(l => l.Name)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.Property(l => l.Latitude)
            .IsRequired();

        builder.Property(l => l.Longitude)
            .IsRequired();
    }
}
