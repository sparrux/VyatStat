using FluentValidation;
using Tracker.Application.Contracts.Groups.Requests;

namespace Tracker.Application.Validators.Group;

sealed class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        Include(new GroupNameValidator<CreateGroupRequest>(x => x.Name));
    }
}
