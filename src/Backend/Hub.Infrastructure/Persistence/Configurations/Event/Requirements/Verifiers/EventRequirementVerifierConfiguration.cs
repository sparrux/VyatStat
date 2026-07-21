using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifiers;

sealed class EventRequirementVerifierConfiguration : IEntityTypeConfiguration<EventRequirementVerifier>
{
    public void Configure(EntityTypeBuilder<EventRequirementVerifier> builder)
    {
        builder.ToTable("event_requirement_verifier");
        
        builder.ConfigureAuditable();
        
        builder.HasDiscriminator<string>("verifier_type")
            .HasValue<EventRequirementParticipantVerifier>("participant")
            .HasValue<EventRequirementRoleVerifier>("role");
        
        builder.Property(x => x.IsRequired)
            .IsRequired();
        
        builder.HasOne(x => x.Requirement)
            .WithMany(x => x.Verifiers)
            .HasForeignKey(x => x.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Verifications)
            .WithOne(x => x.Verifier)
            .HasForeignKey(x => x.VerifierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(x => x.Verifications, "_verifications");
    }
}