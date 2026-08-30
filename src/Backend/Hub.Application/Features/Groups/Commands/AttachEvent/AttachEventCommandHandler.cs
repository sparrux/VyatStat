using Ardalis.Result;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Pipelines;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Groups.Commands.AttachEvent;

sealed class AttachEventCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<AttachEventCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(AttachEventCommand request, CancellationToken cancellationToken)
    {
        var evt = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == request.EventId, cancellationToken);
        if (evt is null) return Result.NotFound("Event not found");
        
        var group = await dbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
        if (group is null) return Result.NotFound("Group not found");
        
        var alreadyAttached = await dbContext.GroupEvent
            .AnyAsync(x => 
                x.GroupId == request.GroupId 
                && x.EventId == request.EventId, 
                cancellationToken);
        
        if (alreadyAttached) return Result.Error("Event already attached to group");
        
        var attach = group.AttachEvent(evt);
        if (!attach.IsSuccess) return attach.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(evt.Id));
    }
}