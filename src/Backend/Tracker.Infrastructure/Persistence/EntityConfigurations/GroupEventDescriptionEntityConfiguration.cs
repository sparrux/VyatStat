using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventDescriptionEntityConfiguration : IEntityTypeConfiguration<GroupEventDescription>
{
    public void Configure(EntityTypeBuilder<GroupEventDescription> builder)
    {
        builder.ToTable("group_event_descriptions");

        builder.ConfigureEntity();

        builder.Property(d => d.Text)
            .IsRequired();

        builder.Property(d => d.Format)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(d => d.EventId);

        builder.HasIndex(d => d.EventId)
            .IsUnique();
    }
}
