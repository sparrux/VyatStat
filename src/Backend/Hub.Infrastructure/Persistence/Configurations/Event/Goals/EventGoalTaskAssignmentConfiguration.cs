using Hub.Domain.Events.Goals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Goals;

sealed class EventGoalTaskAssignmentConfiguration : IEntityTypeConfiguration<EventGoalTaskAssignment>
{
    public void Configure(EntityTypeBuilder<EventGoalTaskAssignment> builder)
    {
        builder.ToTable("event_goal_task_assignment");
        
        builder.ConfigureEntity();
        
        builder.HasOne(x => x.Task)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.ParticipantAssignment)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.ParticipantAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TaskId);
        builder.HasIndex(x => x.ParticipantAssignmentId);
    }
}