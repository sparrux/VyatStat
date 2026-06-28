using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class LocationEntityConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.ConfigureAuditable();

        builder.Property(l => l.Name)
            .HasMaxLength(300);

        builder.ConfigureReadOnlyProperty(l => l.Name);

        builder.Property(l => l.Latitude)
            .IsRequired();

        builder.Property(l => l.Longitude)
            .IsRequired();
    }
}
