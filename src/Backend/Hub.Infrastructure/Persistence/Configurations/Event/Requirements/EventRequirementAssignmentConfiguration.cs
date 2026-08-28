using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements;

public sealed class EventRequirementAssignmentConfiguration
    : IEntityTypeConfiguration<EventRequirementAssignment>
{
    public void Configure(EntityTypeBuilder<EventRequirementAssignment> builder)
    {
        builder.ToTable("event_requirement_assignment");

        builder.ConfigureAuditable();

        builder.HasOne(x => x.AssignParticipant)
            .WithMany(x => x.Requirements)
            .HasForeignKey(x => x.AssignParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Requirement)
            .WithMany(r => r.Assignments)
            .HasForeignKey(c => c.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Verifications)
            .WithOne(x => x.RequirementAssignment)
            .HasForeignKey(x => x.RequirementAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(x => x.Verifications, "_verifications");

        builder.HasIndex(x => x.AssignParticipantId);
        builder.HasIndex(x => x.RequirementId);
    }
}
