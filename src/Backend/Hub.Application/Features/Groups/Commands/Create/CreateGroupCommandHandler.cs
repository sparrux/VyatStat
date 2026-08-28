using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Groups.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Groups;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Groups.Commands.Create;

sealed class CreateGroupCommandHandler(
    HubDbContext dbContext,
    IUserContext userContext
) : IRequestHandler<CreateGroupCommand, GroupSummaryResponse>
{
    public async Task<Result<GroupSummaryResponse>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var actor = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userContext.UserId, cancellationToken);
        if (actor is null) return Result.NotFound("User not found");
        
        var group = Group.Create(request.Name, actor);
        if (!group.IsSuccess) return group.Map();
        
        await dbContext.AddAsync(group.Value, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new GroupSummaryResponse(
            group.Value.Id,
            group.Value.Name,
            group.Value.Members.Count));
    }
}