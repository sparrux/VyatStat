using Hub.Domain.Events.Goals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Goals;

public sealed class EventGoalConfiguration : IEntityTypeConfiguration<EventGoal>
{
    public void Configure(EntityTypeBuilder<EventGoal> builder)
    {
        builder.ToTable("event_goal");

        builder.ConfigureAuditable();
        
        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(t => t.Event)
            .WithMany(e => e.Goals)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(t => t.Tasks)
            .WithOne(e => e.Goal)
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(x => x.Tasks, "_tasks");

        builder.HasIndex(t => t.EventId);
    }
}
