using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.ConfigureAuditable();

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.ConfigureReadOnlyProperty(u => u.Id);

        builder.Property(u => u.Nickname)
            .HasMaxLength(100)
            .IsRequired();

        builder.ConfigureReadOnlyProperty(u => u.Nickname);

        builder.HasIndex(u => u.Nickname)
            .IsUnique();

        builder.ConfigureCollection(u => u.Memberships, "_memberships");
    }
}
