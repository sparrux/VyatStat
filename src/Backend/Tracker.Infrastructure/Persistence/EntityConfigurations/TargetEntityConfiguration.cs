using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class TargetEntityConfiguration : IEntityTypeConfiguration<Target>
{
    public void Configure(EntityTypeBuilder<Target> builder)
    {
        builder.ToTable("targets");

        builder.ConfigureAuditable();

        builder.HasDiscriminator<string>("discriminator")
            .HasValue<GroupEventTarget>("group_event_target");

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.IsAchieved)
            .IsRequired();

        builder.Property(t => t.CurrentValue)
            .IsRequired();

        builder.Property(t => t.TargetValue)
            .IsRequired();
    }
}
