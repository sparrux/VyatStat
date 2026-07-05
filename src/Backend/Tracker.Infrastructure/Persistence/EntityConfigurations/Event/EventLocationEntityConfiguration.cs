using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Event;

public sealed class EventLocationEntityConfiguration : IEntityTypeConfiguration<EventLocation>
{
    public void Configure(EntityTypeBuilder<EventLocation> builder)
    {
        builder.ToTable("event_location");

        builder.ConfigureAuditable();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired(false);
        
        builder.Property(x => x.Latitude);
        builder.Property(x => x.Longitude);

        builder.HasOne(x => x.Event)
            .WithOne(x => x.Location)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.EventId);
    }
}
