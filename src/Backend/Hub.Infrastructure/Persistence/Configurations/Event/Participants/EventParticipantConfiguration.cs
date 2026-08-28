using Hub.Domain.Events.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Participants;

public sealed class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
{
    public void Configure(EntityTypeBuilder<EventParticipant> builder)
    {
        builder.ToTable("event_participant");

        builder.ConfigureAuditable();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Assesses)
            .WithOne(x => x.Assessor)
            .HasForeignKey(x => x.AssessorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Rates)
            .WithOne(x => x.Rater)
            .HasForeignKey(x => x.RaterId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Reports)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Roles)
            .WithOne(x => x.Participant)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Requirements)
            .WithOne(x => x.AssignParticipant)
            .HasForeignKey(x => x.AssignParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Tasks)
            .WithOne(x => x.ParticipantAssignment)
            .HasForeignKey(x => x.ParticipantAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(i => i.Assesses, "_assesses");
        builder.ConfigureCollection(i => i.Rates, "_rates");
        builder.ConfigureCollection(i => i.Reports, "_reports");
        builder.ConfigureCollection(i => i.Roles, "_roles");
        builder.ConfigureCollection(i => i.Tasks, "_tasks");
        builder.ConfigureCollection(i => i.Requirements, "_requirements");

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => new { x.EventId, x.UserId })
            .IsUnique();
    }
}
