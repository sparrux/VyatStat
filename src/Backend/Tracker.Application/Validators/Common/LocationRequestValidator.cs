using FluentValidation;
using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Validators.Common;

public sealed class LocationRequestValidator : AbstractValidator<LocationRequest>
{
    const int NameMaxLength = 300;
    const double MinLatitude = -90;
    const double MaxLatitude = 90;
    const double MinLongitude = -180;
    const double MaxLongitude = 180;

    public LocationRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(NameMaxLength)
            .WithMessage($"Location name must be at most {NameMaxLength} characters long")
            .When(x => x.Name is not null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(MinLatitude, MaxLatitude)
            .WithMessage($"Latitude must be between {MinLatitude} and {MaxLatitude}");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(MinLongitude, MaxLongitude)
            .WithMessage($"Longitude must be between {MinLongitude} and {MaxLongitude}");
    }
}
