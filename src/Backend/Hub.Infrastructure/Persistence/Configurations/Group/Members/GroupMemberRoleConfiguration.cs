using Hub.Domain.Groups.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group.Members;

sealed class GroupMemberRoleConfiguration : IEntityTypeConfiguration<GroupMemberRole>
{
    public void Configure(EntityTypeBuilder<GroupMemberRole> builder)
    {
        builder.ToTable("group_member_role");

        builder.ConfigureEntity();
        
        builder.HasOne(x => x.Role)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Member)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RoleId);
        builder.HasIndex(x => x.MemberId);
        builder.HasIndex(x => new { x.RoleId, x.MemberId })
            .IsUnique();
    }
}