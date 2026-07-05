using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Application.Validators.Common;

namespace Tracker.Application.Validators.Event;

sealed class UpdateEventDescriptionRequestValidator : AbstractValidator<UpdateEventDescriptionRequest>
{
    public UpdateEventDescriptionRequestValidator()
    {
        RuleFor(x => x.NewDescription)
            .SetValidator(new EventDescriptionRequestValidator());
    }
}
