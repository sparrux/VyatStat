using Hub.Domain.Groups.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group.Training;

sealed class TrainingModuleSkillConfiguration : IEntityTypeConfiguration<TrainingSkill>
{
    public void Configure(EntityTypeBuilder<TrainingSkill> builder)
    {
        builder.ToTable("group_training_module_skill");
        
        builder.ConfigureAuditable();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasMaxLength(300);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Module)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ModuleId);
    }
}