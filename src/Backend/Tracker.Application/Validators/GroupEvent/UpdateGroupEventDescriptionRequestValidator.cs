using FluentValidation;
using Tracker.Application.Contracts.Event.Requests;
using Tracker.Application.Validators.Common;

namespace Tracker.Application.Validators.GroupEvent;

sealed class UpdateGroupEventDescriptionRequestValidator : AbstractValidator<UpdateGroupEventDescriptionRequest>
{
    public UpdateGroupEventDescriptionRequestValidator()
    {
        RuleFor(x => x.NewDescription)
            .SetValidator(new FormatTextRequestValidator());
    }
}
