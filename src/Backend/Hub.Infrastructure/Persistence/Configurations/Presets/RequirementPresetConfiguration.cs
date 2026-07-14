using Hub.Domain.Presets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations.Presets;

public sealed class RequirementPresetConfiguration : IEntityTypeConfiguration<RequirementPreset>
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

        builder.Property(r => r.VerificationMode)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.VerificationMode);
    }
}