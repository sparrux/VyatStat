using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventInviteeRequirementCompletionEntityConfiguration
    : IEntityTypeConfiguration<GroupEventInviteeRequirementCompletion>
{
    public void Configure(EntityTypeBuilder<GroupEventInviteeRequirementCompletion> builder)
    {
        builder.ToTable("group_event_invitee_requirement_completions");

        builder.ConfigureAuditable();

        builder.Property(c => c.CompletionStatus)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(c => c.InviteeId);
        builder.ConfigureReadOnlyProperty(c => c.RequirementId);

        builder.HasOne(c => c.Invitee)
            .WithMany(i => i.RequirementCompletions)
            .HasForeignKey(c => c.InviteeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Requirement)
            .WithMany(r => r.Completions)
            .HasForeignKey(c => c.RequirementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.InviteeId, c.RequirementId })
            .IsUnique();
    }
}
