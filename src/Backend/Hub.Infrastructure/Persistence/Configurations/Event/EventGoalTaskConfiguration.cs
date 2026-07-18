using Hub.Domain.Events.Goals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

sealed class EventGoalTaskConfiguration : IEntityTypeConfiguration<EventGoalTask>
{
    public void Configure(EntityTypeBuilder<EventGoalTask> builder)
    {
        builder.ToTable("event_goal_task");

        builder.ConfigureEntity();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.HasOne(x => x.Goal)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.GoalId);
    }
}