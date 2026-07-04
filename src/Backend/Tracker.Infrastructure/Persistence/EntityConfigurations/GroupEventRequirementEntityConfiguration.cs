using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventRequirementEntityConfiguration : IEntityTypeConfiguration<GroupEventRequirement>
{
    public void Configure(EntityTypeBuilder<GroupEventRequirement> builder)
    {
        builder.ToTable("group_event_requirements");

        builder.ConfigureEntity();

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.IsMandatory)
            .IsRequired();

        builder.Property(r => r.SortOrder)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(r => r.EventId);

        builder.HasOne(r => r.Event)
            .WithMany(e => e.Requirements)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(r => r.Completions, "_completions");
        
        builder.HasMany(x => x.Completions)
            .WithOne(x => x.Requirement)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.EventId, r.SortOrder })
            .IsUnique();
    }
}
