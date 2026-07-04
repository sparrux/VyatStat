using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEventLocationEntityConfiguration : IEntityTypeConfiguration<GroupEventLocation>
{
    public void Configure(EntityTypeBuilder<GroupEventLocation> builder)
    {
        builder.ToTable("group_event_locations");

        builder.ConfigureEntity();

        builder.ConfigureReadOnlyProperty(l => l.EventId);

        builder.HasOne(l => l.Location)
            .WithMany()
            .HasForeignKey(l => l.LocationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(l => l.EventId)
            .IsUnique();
    }
}
