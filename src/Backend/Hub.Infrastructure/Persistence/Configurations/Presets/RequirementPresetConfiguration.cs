using Hub.Domain.Presets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Presets;

sealed class RequirementPresetConfiguration : IEntityTypeConfiguration<RequirementPreset>
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
    }
}