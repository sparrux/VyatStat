using Hub.Domain.Events.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Event.Training;

sealed class EventTrainingRatingConfiguration : IEntityTypeConfiguration<EventTrainingRating>
{
    public void Configure(EntityTypeBuilder<EventTrainingRating> builder)
    {
        builder.ToTable("event_training_rating");
        
        builder.ConfigureAuditable();

        builder.HasOne(x => x.Rater)
            .WithMany(x => x.Rates)
            .HasForeignKey(x => x.RaterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Rating)
            .WithOne()
            .HasForeignKey<EventTrainingRating>(x => x.RatingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Rates)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RaterId);
        builder.HasIndex(x => x.RatingId);
    }
}