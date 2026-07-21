using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event;

public sealed class EventConfiguration : IEntityTypeConfiguration<Domain.Events.Event>
{
    public void Configure(EntityTypeBuilder<Domain.Events.Event> builder)
    {
        builder.ToTable("event");

        builder.ConfigureEntity();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.ComplexProperty(x => x.Description, description =>
        {
            description.Property(x => x.Text)
                .IsRequired();
            
            description.Property(x => x.Format)
                .HasConversion<string>()
                .IsRequired();
        });

        builder.HasOne(x => x.Location)
            .WithOne(x => x.Event)
            .HasForeignKey<Domain.Events.EventLocation>(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ComplexProperty(x => x.DatesRange, dates =>
        {
            dates.Property(x => x.StartDate)
                .IsRequired();

            dates.Property(x => x.EndDate)
                .IsRequired();
        });

        builder.Property(e => e.State)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasMany(x => x.Roles)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Goals)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Participants)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.GroupEvents)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Requirements)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(e => e.Roles, "_roles");
        builder.ConfigureCollection(e => e.Goals, "_goals");
        builder.ConfigureCollection(e => e.Participants, "_participants");
        builder.ConfigureCollection(e => e.GroupEvents, "_groupEvents");
        builder.ConfigureCollection(e => e.Requirements, "_requirements");

        builder.HasIndex(x => x.State);
    }
}
