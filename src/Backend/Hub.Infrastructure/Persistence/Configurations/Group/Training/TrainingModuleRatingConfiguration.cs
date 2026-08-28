using Hub.Domain.Groups.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Group.Training;

sealed class TrainingModuleRatingConfiguration : IEntityTypeConfiguration<TrainingRating>
{
    public void Configure(EntityTypeBuilder<TrainingRating> builder)
    {
        builder.ToTable("group_training_module_rating");
        
        builder.ConfigureAuditable();

        builder.Property(x => x.Rating)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Ratings)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Module)
            .WithMany(x => x.Ratings)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ModuleId);
    }
}