using Hub.Domain.Groups.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group.Members;

sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
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
        
        builder.HasMany(m => m.Roles)
            .WithOne(g => g.Member)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(x => x.Roles, "_roles");

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.GroupId);
    }
}
