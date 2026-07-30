using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group;

sealed class GroupConfiguration : IEntityTypeConfiguration<Domain.Groups.Group>
{
    public void Configure(EntityTypeBuilder<Domain.Groups.Group> builder)
    {
        builder.ToTable("group");

        builder.ConfigureEntity();

        builder.Property(g => g.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.HasMany(x => x.Roles)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.GroupEvents)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Members)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Modules)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(g => g.Roles, "_roles");
        builder.ConfigureCollection(g => g.Members, "_members");
        builder.ConfigureCollection(g => g.Modules, "_modules");
        builder.ConfigureCollection(g => g.GroupEvents, "_groupEvents");
    }
}
