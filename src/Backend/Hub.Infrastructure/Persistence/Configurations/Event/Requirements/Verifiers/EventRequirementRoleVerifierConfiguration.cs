using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifiers;

sealed class EventRequirementRoleVerifierConfiguration : IEntityTypeConfiguration<EventRequirementRoleVerifier>
{
    public void Configure(EntityTypeBuilder<EventRequirementRoleVerifier> builder)
    {
        builder.HasOne(x => x.Verifier)
            .WithMany()
            .HasForeignKey(x => x.VerifierId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.VerifierId)
            .HasColumnName("verifier_role_id");
    }
}