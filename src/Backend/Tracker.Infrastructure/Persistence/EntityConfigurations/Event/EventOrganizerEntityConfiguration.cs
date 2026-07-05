using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Event;

public sealed class EventOrganizerEntityConfiguration : IEntityTypeConfiguration<EventOrganizer>
{
    public void Configure(EntityTypeBuilder<EventOrganizer> builder)
    {
        builder.ToTable("event_organizer");

        builder.ConfigureAuditable();
        
        builder.HasOne(o => o.User)
            .WithMany(x => x.Organizers)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Event)
            .WithMany(e => e.Organizers)
            .HasForeignKey(o => o.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EventId);
    }
}
