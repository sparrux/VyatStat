using Hub.Domain.Events.Requirements.VerificationRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Rules;

sealed class ContributionPaidVerificationRuleConfiguration : IEntityTypeConfiguration<ContributionPaidVerificationRule>
{
    public void Configure(EntityTypeBuilder<ContributionPaidVerificationRule> builder)
    {
        builder.Property(x => x.Code)
            .HasMaxLength(100);
    }
}