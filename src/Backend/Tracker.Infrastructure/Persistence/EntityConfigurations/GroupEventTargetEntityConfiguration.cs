using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventTargetEntityConfiguration : IEntityTypeConfiguration<GroupEventTarget>
{
    public void Configure(EntityTypeBuilder<GroupEventTarget> builder)
    {
        builder.ToTable("group_event_targets");

        builder.ConfigureAuditable();

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.IsAchieved)
            .IsRequired();

        builder.Property(t => t.CurrentValue)
            .IsRequired();

        builder.Property(t => t.TargetValue)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(t => t.EventId);

        builder.HasOne(t => t.Event)
            .WithMany(e => e.Targets)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.EventId);
    }
}
