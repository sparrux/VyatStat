using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.User;

sealed class UserConfiguration : IEntityTypeConfiguration<Domain.User>
{
    public void Configure(EntityTypeBuilder<Domain.User> builder)
    {
        builder.ToTable("user");

        builder.ConfigureAuditable();

        builder.Property(u => u.Nickname)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Nickname)
            .IsUnique();
        
        builder.HasMany(x => x.Ratings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Skills)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Participants)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Memberships)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(u => u.Skills, "_skills");
        builder.ConfigureCollection(u => u.Ratings, "_ratings");
        builder.ConfigureCollection(u => u.Participants, "_participants");
        builder.ConfigureCollection(u => u.Memberships, "_memberships");
    }
}
