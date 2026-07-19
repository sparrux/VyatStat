using Hub.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

sealed class EventRoleConfiguration : IEntityTypeConfiguration<EventRole>
{
    public void Configure(EntityTypeBuilder<EventRole> builder)
    {
        builder.ToTable("event_role");
        
        builder.ConfigureEntity();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsSealed)
            .IsRequired();
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Participants)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ConfigureCollection(e => e.Participants, "_participants");

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => new { x.EventId, x.Name })
            .IsUnique();
    }
}
