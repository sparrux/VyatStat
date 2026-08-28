using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifications;

sealed class EventRequirementVerificationConfiguration : IEntityTypeConfiguration<EventRequirementVerification>
{
    public void Configure(EntityTypeBuilder<EventRequirementVerification> builder)
    {
        builder.ToTable("event_requirement_verification");
        
        builder.ConfigureEntity();
        
        builder.HasDiscriminator<string>("verification_type")
            .HasValue<EventRequirementParticipantVerification>("participant")
            .HasValue<EventRequirementRoleVerification>("role");
        
        builder.HasOne(x => x.Verifier)
            .WithMany(x => x.Verifications)
            .HasForeignKey(x => x.VerifierId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.RequirementAssignment)
            .WithMany(x => x.Verifications)
            .HasForeignKey(x => x.RequirementAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.VerifierId);
        builder.HasIndex(x => x.RequirementAssignmentId);
    }
}