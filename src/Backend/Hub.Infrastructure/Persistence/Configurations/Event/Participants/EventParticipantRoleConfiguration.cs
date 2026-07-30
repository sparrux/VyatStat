using Hub.Domain.Events.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Participants;

sealed class EventParticipantRoleConfiguration : IEntityTypeConfiguration<EventParticipantRole>
{
    public void Configure(EntityTypeBuilder<EventParticipantRole> builder)
    {
        builder.ToTable("event_participant_role");

        builder.ConfigureEntity();
        
        builder.HasOne(x => x.Role)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Participant)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RoleId);
        builder.HasIndex(x => x.ParticipantId);
        builder.HasIndex(x => new { x.RoleId, x.ParticipantId })
            .IsUnique();
    }
}