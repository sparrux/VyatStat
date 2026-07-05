using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Event;

public sealed class EventDescriptionEntityConfiguration : IEntityTypeConfiguration<EventDescription>
{
    public void Configure(EntityTypeBuilder<EventDescription> builder)
    {
        builder.ToTable("event_description");

        builder.ConfigureEntity();

        builder.Property(d => d.Text)
            .IsRequired();

        builder.Property(d => d.Format)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasOne(x => x.Event)
            .WithOne(x => x.Description)
            .HasForeignKey<EventDescription>(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.EventId)
            .IsUnique();
    }
}
