using Hub.Domain.Events.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Reports;

sealed class EventReportConfiguration : IEntityTypeConfiguration<EventReport>
{
    public void Configure(EntityTypeBuilder<EventReport> builder)
    {
        builder.ToTable("event_report");
        
        builder.ConfigureAuditable();

        builder.Property(x => x.Title)
            .IsRequired();

        builder.ComplexProperty(x => x.Body, body =>
        {
            body.Property(x => x.Text)
                .IsRequired()
                .HasColumnName("text");

            body.Property(x => x.Format)
                .HasConversion<string>()
                .HasColumnName("format")
                .IsRequired();
        });
        
        builder.HasOne(x => x.Author)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => x.AuthorId);
    }
}