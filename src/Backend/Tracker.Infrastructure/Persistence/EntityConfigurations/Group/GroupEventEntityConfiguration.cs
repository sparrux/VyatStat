using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Group;

public sealed class GroupEventEntityConfiguration : IEntityTypeConfiguration<GroupEvent>
{
    public void Configure(EntityTypeBuilder<GroupEvent> builder)
    {
        builder.ToTable("group_event");

        builder.ConfigureAuditable();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.GroupEvents)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Group)
            .WithMany(x => x.GroupEvents)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}