using FluentValidation;
using Tracker.Application.Contracts.Users.Requests;

namespace Tracker.Application.Validators.User;

sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        Include(new NicknameValidator<UpdateUserRequest>(x => x.NewNickname));
    }
}
