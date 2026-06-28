using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations;

public sealed class GroupMemberEntityConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable("group_members");

        builder.ConfigureAuditable();

        builder.ConfigureReadOnlyProperty(m => m.UserId);
        builder.ConfigureReadOnlyProperty(m => m.GroupId);

        builder.HasOne(m => m.User)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.UserId, m.GroupId })
            .IsUnique();
    }
}
