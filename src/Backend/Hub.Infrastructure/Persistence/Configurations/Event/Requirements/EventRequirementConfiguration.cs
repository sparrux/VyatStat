using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Requirements;

public sealed class EventRequirementConfiguration : IEntityTypeConfiguration<EventRequirement>
{
    public void Configure(EntityTypeBuilder<EventRequirement> builder)
    {
        builder.ToTable("event_requirement");

        builder.ConfigureAuditable();

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.HasOne(r => r.Event)
            .WithMany(e => e.Requirements)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Assignments)
            .WithOne(x => x.Requirement)
            .HasForeignKey(x => x.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Verifiers)
            .WithOne(x => x.Requirement)
            .HasForeignKey(x => x.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(r => r.Verifiers, "_verifiers");
        builder.ConfigureCollection(r => r.Assignments, "_assignments");

        builder.HasIndex(x => x.EventId);
    }
}
