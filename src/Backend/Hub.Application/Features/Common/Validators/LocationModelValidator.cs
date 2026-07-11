using FluentValidation;
using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Common.Validators;

sealed class LocationModelValidator : AbstractValidator<LocationModel>
{
    const double MinLatitude = -90;
    const double MaxLatitude = 90;
    const double MinLongitude = -180;
    const double MaxLongitude = 180;

    public LocationModelValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(MinLatitude, MaxLatitude)
            .WithMessage($"Latitude must be between {MinLatitude} and {MaxLatitude}");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(MinLongitude, MaxLongitude)
            .WithMessage($"Longitude must be between {MinLongitude} and {MaxLongitude}");
    }
}