using Hub.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group;

public sealed class GroupEventConfiguration : IEntityTypeConfiguration<GroupEvent>
{
    public void Configure(EntityTypeBuilder<GroupEvent> builder)
    {
        builder.ToTable("group_event");

        builder.ConfigureEntity();

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