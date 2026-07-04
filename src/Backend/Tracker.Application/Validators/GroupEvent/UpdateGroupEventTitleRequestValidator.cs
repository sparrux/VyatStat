using FluentValidation;
using Tracker.Application.Contracts.GroupEvents.Requests;

namespace Tracker.Application.Validators.GroupEvent;

sealed class UpdateGroupEventTitleRequestValidator : AbstractValidator<UpdateGroupEventTitleRequest>
{
    public UpdateGroupEventTitleRequestValidator()
    {
        Include(new GroupEventTitleValidator<UpdateGroupEventTitleRequest>(x => x.NewTitle));
    }
}
