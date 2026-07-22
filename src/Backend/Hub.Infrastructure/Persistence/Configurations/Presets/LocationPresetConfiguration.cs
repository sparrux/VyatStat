using Hub.Domain.Presets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Presets;

sealed class LocationPresetConfiguration : IEntityTypeConfiguration<LocationPreset>
{
    public void Configure(EntityTypeBuilder<LocationPreset> builder)
    {
        builder.ToTable("location_preset");

        builder.ConfigureEntity();

        builder.Property(l => l.Name)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.ComplexProperty(x => x.Coordinates, coordinates =>
        {
            coordinates.Property(x => x.Latitude)
                .IsRequired();
            
            coordinates.Property(x => x.Longitude)
                .IsRequired();
        });
    }
}
