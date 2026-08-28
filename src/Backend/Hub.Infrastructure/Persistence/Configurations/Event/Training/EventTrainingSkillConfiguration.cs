using Hub.Domain.Events.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Training;

sealed class EventTrainingSkillConfiguration : IEntityTypeConfiguration<EventTrainingSkill>
{
    public void Configure(EntityTypeBuilder<EventTrainingSkill> builder)
    {
        builder.ToTable("event_training_skill");
        
        builder.ConfigureAuditable();

        builder.HasOne(x => x.Skill)
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Assessor)
            .WithMany(x => x.Assesses)
            .HasForeignKey(x => x.AssessorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.SkillsAssessments)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => x.AssessorId);
    }
}