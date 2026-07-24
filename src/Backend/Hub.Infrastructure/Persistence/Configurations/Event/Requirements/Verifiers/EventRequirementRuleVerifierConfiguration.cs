using Hub.Domain.Events.Requirements;
using Hub.Domain.Events.Requirements.VerificationRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifiers;

sealed class EventRequirementRuleVerifierConfiguration : IEntityTypeConfiguration<EventRequirementRuleVerifier>
{
    public void Configure(EntityTypeBuilder<EventRequirementRuleVerifier> builder)
    {
        builder.HasOne(x => x.Verifier)
            .WithOne(x => x.Verifier)
            .HasForeignKey<EventRequirementVerificationRule>(x => x.VerifierId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.VerifierId)
            .HasColumnName("verifier_rule_id");
    }
}