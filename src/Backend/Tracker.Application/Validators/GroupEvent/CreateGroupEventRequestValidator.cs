using FluentValidation;
using Tracker.Application.Contracts.GroupEvents.Requests;
using Tracker.Application.Validators.Common;

namespace Tracker.Application.Validators.GroupEvent;

sealed class CreateGroupEventRequestValidator : AbstractValidator<CreateGroupEventRequest>
{
    public CreateGroupEventRequestValidator()
    {
        Include(new GroupEventTitleValidator<CreateGroupEventRequest>(x => x.Title));
        Include(new GroupEventDatesValidator<CreateGroupEventRequest>(x => x.StartDate, x => x.EndDate));

        RuleFor(x => x.Description)
            .SetValidator(new FormatTextRequestValidator());

        RuleFor(x => x.Location)
            .SetValidator(new LocationRequestValidator())
            .When(x => x.Location is not null);
    }
}
