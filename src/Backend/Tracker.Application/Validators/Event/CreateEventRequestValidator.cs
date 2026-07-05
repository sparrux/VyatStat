using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Application.Validators.Common;

namespace Tracker.Application.Validators.Event;

sealed class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        Include(new EventTitleValidator<CreateEventRequest>(x => x.Title));
        Include(new EventDatesValidator<CreateEventRequest>(x => x.StartDate, x => x.EndDate));

        RuleFor(x => x.Description)
            .SetValidator(new EventDescriptionRequestValidator());

        RuleFor(x => x.Location)
            .SetValidator(new EventLocationRequestValidator())
            .When(x => x.Location is not null);
    }
}
