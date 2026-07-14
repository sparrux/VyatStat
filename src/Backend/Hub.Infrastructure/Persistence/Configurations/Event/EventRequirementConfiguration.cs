using Hub.Domain.Events.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

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

        builder.Property(r => r.IsMandatory)
            .IsRequired();

        builder.Property(r => r.VerificationMode)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(r => r.Event)
            .WithMany(e => e.Requirements)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(r => r.Completions, "_completions");
        
        builder.HasMany(x => x.Completions)
            .WithOne(x => x.Requirement)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => x.VerificationMode);
    }
}
