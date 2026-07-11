using Hub.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

public sealed class EventGoalConfiguration : IEntityTypeConfiguration<EventGoal>
{
    public void Configure(EntityTypeBuilder<EventGoal> builder)
    {
        builder.ToTable("event_goal");

        builder.ConfigureAuditable();
        
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.ComplexProperty(x => x.State, state =>
        {
            state.Property(x => x.CurrentValue)
                .IsRequired();
            
            state.Property(x => x.TargetValue)
                .IsRequired();
        });

        builder.HasOne(t => t.Event)
            .WithMany(e => e.Goals)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.EventId);
    }
}
