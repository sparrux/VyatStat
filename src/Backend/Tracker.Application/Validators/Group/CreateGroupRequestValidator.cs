using FluentValidation;
using Tracker.Application.Contracts.Group.Requests;

namespace Tracker.Application.Validators.Group;

public sealed class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        Include(new GroupNameValidator<CreateGroupRequest>(x => x.Name));
    }
}
