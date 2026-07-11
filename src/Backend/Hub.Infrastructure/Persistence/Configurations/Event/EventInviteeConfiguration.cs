using Hub.Domain.Events.Invitees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

public sealed class EventInviteeConfiguration : IEntityTypeConfiguration<EventInvitee>
{
    public void Configure(EntityTypeBuilder<EventInvitee> builder)
    {
        builder.ToTable("event_invitee");

        builder.ConfigureAuditable();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Invitees)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Invitees)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Invitees)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.RsvpStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.AdmissionStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasMany(x => x.RequirementCompletions)
            .WithOne(x => x.Invitee)
            .HasForeignKey(x => x.InviteeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(i => i.RequirementCompletions, "_requirementCompletions");

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EventId);
    }
}
