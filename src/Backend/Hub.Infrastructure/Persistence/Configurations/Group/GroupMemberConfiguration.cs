using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group;

sealed class GroupMemberConfiguration : IEntityTypeConfiguration<Domain.Groups.GroupMember>
{
    public void Configure(EntityTypeBuilder<Domain.Groups.GroupMember> builder)
    {
        builder.ToTable("group_member");

        builder.ConfigureAuditable();

        builder.HasOne(m => m.User)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.GroupId);
    }
}
