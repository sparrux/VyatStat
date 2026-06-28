using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupEntityConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");

        builder.ConfigureAuditable();

        builder.Property(g => g.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.ConfigureCollection(g => g.Members, "_members");
        builder.ConfigureCollection(g => g.Events, "_events");
    }
}
