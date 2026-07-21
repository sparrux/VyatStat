using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifiers;

sealed class EventRequirementParticipantVerifierConfiguration : IEntityTypeConfiguration<EventRequirementParticipantVerifier>
{
    public void Configure(EntityTypeBuilder<EventRequirementParticipantVerifier> builder)
    {
        builder.HasOne(x => x.Verifier)
            .WithMany()
            .HasForeignKey(x => x.VerifierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.VerifierId)
            .HasColumnName("verifier_participant_id");
    }
}