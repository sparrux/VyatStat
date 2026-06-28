using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventOrganizerEntityConfiguration : IEntityTypeConfiguration<GroupEventOrganizer>
{
    public void Configure(EntityTypeBuilder<GroupEventOrganizer> builder)
    {
        builder.ToTable("group_event_organizers");

        builder.ConfigureEntity();

        builder.ConfigureReadOnlyProperty(o => o.UserId);
        builder.ConfigureReadOnlyProperty(o => o.EventId);

        builder.HasOne(o => o.Event)
            .WithMany(e => e.Organizers)
            .HasForeignKey(o => o.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(o => new { o.UserId, o.EventId })
            .IsUnique();
    }
}
