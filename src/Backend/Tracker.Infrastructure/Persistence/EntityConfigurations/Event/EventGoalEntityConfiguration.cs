using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Event;

public sealed class EventGoalEntityConfiguration : IEntityTypeConfiguration<EventGoal>
{
    public void Configure(EntityTypeBuilder<EventGoal> builder)
    {
        builder.ToTable("event_goal");

        builder.ConfigureAuditable();
        
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.CurrentValue)
            .IsRequired();

        builder.Property(t => t.TargetValue)
            .IsRequired();

        builder.HasOne(t => t.Event)
            .WithMany(e => e.Goals)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.EventId);
    }
}
