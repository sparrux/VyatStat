using FluentValidation;
using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Validators.Common;

sealed class ListSelectionRequestValidator : AbstractValidator<ListSelectionRequest>
{
    const int MaxTake = 30;
    
    public ListSelectionRequestValidator()
    {
        RuleFor(x => x.Take)
            .InclusiveBetween(0, 30)
            .WithMessage($"Take must be between 0 and {MaxTake}");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Offset must be greater than or equal to 0");
    }
}
