using FluentValidation;
using Tracker.Application.Contracts.Group.Requests;

namespace Tracker.Application.Validators.Group;

public sealed class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupRequestValidator()
    {
        Include(new GroupNameValidator<UpdateGroupRequest>(x => x.NewName));
    }
}
