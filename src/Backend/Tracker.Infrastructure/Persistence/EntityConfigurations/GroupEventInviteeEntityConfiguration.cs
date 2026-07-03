using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventInviteeEntityConfiguration : IEntityTypeConfiguration<GroupEventInvitee>
{
    public void Configure(EntityTypeBuilder<GroupEventInvitee> builder)
    {
        builder.ToTable("group_event_invitees");

        builder.ConfigureAuditable();

        builder.Property(i => i.RsvpStatus)
            .IsRequired();

        builder.Property(i => i.AdmissionStatus)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(i => i.UserId);
        builder.ConfigureReadOnlyProperty(i => i.EventId);

        builder.HasOne(i => i.Event)
            .WithMany(e => e.Invitees)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.ConfigureCollection(i => i.RequirementCompletions, "_requirementCompletions");

        builder.HasIndex("UserId", nameof(GroupEventInvitee.EventId))
            .IsUnique();
    }
}
