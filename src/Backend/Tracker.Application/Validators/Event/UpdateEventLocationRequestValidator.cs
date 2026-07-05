using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Application.Validators.Common;

namespace Tracker.Application.Validators.Event;

sealed class UpdateEventLocationRequestValidator : AbstractValidator<UpdateEventLocationRequest>
{
    public UpdateEventLocationRequestValidator()
    {
        RuleFor(x => x.NewLocation)
            .SetValidator(new EventLocationRequestValidator())
            .When(x => x.NewLocation is not null);
    }
}
