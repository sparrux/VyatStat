using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

public sealed class EventRequirementCompletionConfiguration
    : IEntityTypeConfiguration<EventRequirementCompletion>
{
    public void Configure(EntityTypeBuilder<EventRequirementCompletion> builder)
    {
        builder.ToTable("event_requirement_completion");

        builder.ConfigureAuditable();

        builder.Property(c => c.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasOne(x => x.Participant)
            .WithMany(x => x.RequirementCompletions)
            .HasForeignKey(x => x.InviteeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Requirement)
            .WithMany(r => r.Completions)
            .HasForeignKey(c => c.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.InviteeId);
        builder.HasIndex(x => x.RequirementId);
        builder.HasIndex(x => new { x.InviteeId, x.RequirementId })
            .IsUnique();
    }
}
