using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;

namespace Tracker.Application.Validators.Event;

sealed class EventFilterRequestValidator : AbstractValidator<EventFilterRequest>
{
    public EventFilterRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => 
                HasValue(x.OrganizerUserId) 
                || HasValue(x.GroupId)
                || HasValue(x.InviteeUserId))
            .WithMessage("Either OrganizerUserId or GroupId or InviteeUserId must be specified.");
    }

    static bool HasValue(Guid? id) =>
        id is { } value && value != Guid.Empty;
}
