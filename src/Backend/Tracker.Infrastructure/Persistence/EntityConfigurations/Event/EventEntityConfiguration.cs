
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Event;

public sealed class EventEntityConfiguration : IEntityTypeConfiguration<Domain.Events.Event>
{
    public void Configure(EntityTypeBuilder<Domain.Events.Event> builder)
    {
        builder.ToTable("event");

        builder.ConfigureAuditable();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.HasOne(x => x.Description)
            .WithOne(x => x.Event)
            .HasForeignKey<EventDescription>(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.State)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasOne(x => x.Location)
            .WithOne(x => x.Event)
            .HasForeignKey<EventLocation>(x => x.EventId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ConfigureCollection(e => e.Goals, "_goals");
        builder.ConfigureCollection(e => e.Invitees, "_invitees");
        builder.ConfigureCollection(e => e.GroupEvents, "_groupEvents");
        builder.ConfigureCollection(e => e.Organizers, "_organizers");
        builder.ConfigureCollection(e => e.Requirements, "_requirements");

        builder.HasIndex(x => x.State);
    }
}
