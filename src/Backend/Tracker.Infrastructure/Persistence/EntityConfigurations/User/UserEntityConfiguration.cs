using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.User;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<Domain.User>
{
    public void Configure(EntityTypeBuilder<Domain.User> builder)
    {
        builder.ToTable("user");

        builder.ConfigureAuditable();

        builder.Property(u => u.Nickname)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Nickname)
            .IsUnique();
        
        builder.HasMany(x => x.Invitees)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Organizers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Memberships)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(u => u.Invitees, "_invitees");
        builder.ConfigureCollection(u => u.Organizers, "_organizers");
        builder.ConfigureCollection(u => u.Memberships, "_memberships");
    }
}
