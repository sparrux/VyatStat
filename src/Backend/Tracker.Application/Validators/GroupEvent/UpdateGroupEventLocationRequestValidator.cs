using FluentValidation;
using Tracker.Application.Contracts.Event.Requests;
using Tracker.Application.Validators.Common;

namespace Tracker.Application.Validators.GroupEvent;

sealed class UpdateGroupEventLocationRequestValidator : AbstractValidator<UpdateGroupEventLocationRequest>
{
    public UpdateGroupEventLocationRequestValidator()
    {
        RuleFor(x => x.NewLocation)
            .SetValidator(new LocationRequestValidator())
            .When(x => x.NewLocation is not null);
    }
}
