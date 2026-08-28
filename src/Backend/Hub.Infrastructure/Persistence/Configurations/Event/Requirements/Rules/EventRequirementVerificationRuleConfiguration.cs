using Hub.Domain.Events.Requirements.VerificationRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Rules;

sealed class EventRequirementVerificationRuleConfiguration : IEntityTypeConfiguration<EventRequirementVerificationRule>
{
    public void Configure(EntityTypeBuilder<EventRequirementVerificationRule> builder)
    {
        builder.ToTable("event_requirement_verification_rule");
        
        builder.ConfigureAuditable();

        builder.HasDiscriminator<string>("rule_type")
            .HasValue<ContributionPaidVerificationRule>("contribution");
        
        builder.HasOne(x => x.Verifier)
            .WithOne(x => x.Verifier)
            .HasForeignKey<EventRequirementVerificationRule>(x => x.VerifierId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.VerifierId);
    }
}