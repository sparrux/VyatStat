using FluentValidation;
using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Validators.Common;

public sealed class PageSelectionRequestValidator : AbstractValidator<PageSelectionRequest>
{
    const int MaxTake = 30;
    
    public PageSelectionRequestValidator()
    {
        RuleFor(x => x.Take)
            .InclusiveBetween(0, 30)
            .WithMessage($"Take must be between 0 and {MaxTake}");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Offset must be greater than or equal to 0");
    }
}
