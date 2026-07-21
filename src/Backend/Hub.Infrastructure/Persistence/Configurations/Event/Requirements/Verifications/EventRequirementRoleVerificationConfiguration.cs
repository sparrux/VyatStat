using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements.Verifications;

sealed class EventRequirementRoleVerificationConfiguration : IEntityTypeConfiguration<EventRequirementRoleVerification>
{
    public void Configure(EntityTypeBuilder<EventRequirementRoleVerification> builder)
    {
        builder.HasOne(x => x.VerifiedBy)
            .WithMany()
            .HasForeignKey(x => x.VerifiedById)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.VerifiedById)
            .HasColumnName("verified_by_role_id");
    }
}