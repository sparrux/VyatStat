using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventEntityConfiguration : IEntityTypeConfiguration<GroupEvent>
{
    public void Configure(EntityTypeBuilder<GroupEvent> builder)
    {
        builder.ToTable("group_events");

        builder.ConfigureAuditable();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(e => e.GroupId);

        builder.HasOne(e => e.Group)
            .WithMany(g => g.Events)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Description)
            .WithOne(d => d.Event)
            .HasForeignKey<GroupEventDescription>(d => d.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Location)
            .WithOne(l => l.Event)
            .HasForeignKey<GroupEventLocation>(l => l.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(e => e.Targets, "_targets");
        builder.ConfigureCollection(e => e.Invitees, "_invitees");
        builder.ConfigureCollection(e => e.Organizers, "_organizers");
        builder.ConfigureCollection(e => e.Requirements, "_requirements");

        builder.HasIndex(e => e.GroupId);
        builder.HasIndex(e => new { e.GroupId, e.StartDate });
        builder.HasIndex(e => e.State);
    }
}
