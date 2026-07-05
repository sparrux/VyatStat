using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Presets;

namespace Tracker.Infrastructure.Persistence.EntityConfigurations.Presets;

public sealed class RequirementPresetEntityConfiguration : IEntityTypeConfiguration<RequirementPreset>
{
    public void Configure(EntityTypeBuilder<RequirementPreset> builder)
    {
        builder.ToTable("requirement_preset");

        builder.ConfigureAuditable();

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.IsMandatory)
            .IsRequired();

        builder.Property(r => r.ConfirmationMode)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.ConfirmationMode);
    }
}