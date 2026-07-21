using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifications;

sealed class EventRequirementParticipantVerificationConfiguration : IEntityTypeConfiguration<EventRequirementParticipantVerification>
{
    public void Configure(EntityTypeBuilder<EventRequirementParticipantVerification> builder)
    {
        builder.HasOne(x => x.VerifiedBy)
            .WithMany()
            .HasForeignKey(x => x.VerifiedById)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.VerifiedById)
            .HasColumnName("verified_by_participant_id");
    }
}