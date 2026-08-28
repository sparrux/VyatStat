using Hub.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group;

sealed class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        builder.ToTable("group_role");
        
        builder.ConfigureAuditable();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsSealed)
            .IsRequired();
        
        builder.HasOne(x => x.Group)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Members)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ConfigureCollection(e => e.Members, "_members");

        builder.HasIndex(x => x.GroupId);
        builder.HasIndex(x => new { x.GroupId, x.Name })
            .IsUnique();
    }
}