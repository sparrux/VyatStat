using Hub.Domain.Groups.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group.Training;

sealed class TrainingModuleConfiguration : IEntityTypeConfiguration<TrainingModule>
{
    public void Configure(EntityTypeBuilder<TrainingModule> builder)
    {
        builder.ToTable("group_training_module");
        
        builder.ConfigureEntity();
        
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Modules)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Skills)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Ratings)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(x => x.Skills, "_skills");
        builder.ConfigureCollection(x => x.Ratings, "_ratings");
    }
}