using Hub.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

public sealed class EventLocationConfiguration : IEntityTypeConfiguration<EventLocation>
{
    public void Configure(EntityTypeBuilder<EventLocation> builder)
    {
        builder.ToTable("event_location");

        builder.ConfigureEntity();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.ComplexProperty(x => x.Coordinates, coordinates =>
        {
            coordinates.Property(x => x.X)
                .HasColumnName("x")
                .IsRequired();
            
            coordinates.Property(x => x.Y)
                .HasColumnName("y")
                .IsRequired();
            
            coordinates.Property(x => x.Epsg)
                .HasColumnName("epsg")
                .IsRequired();
        });

        builder.HasOne(x => x.Event)
            .WithOne(x => x.Location)
            .HasForeignKey<EventLocation>(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.EventId);
    }
}
