using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.GroupMembers.Responses;
using Tracker.Application.Contracts.Groups.Requests;
using Tracker.Application.Contracts.Groups.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Application.Services.Groups;
using Tracker.Domain;
using Tracker.Domain.Groups;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common;
using Tracker.Infrastructure.Persistence.Specs.GroupMembers;
using Tracker.Infrastructure.Persistence.Specs.Groups;

namespace Tracker.Infrastructure.Services.Groups;

public sealed class GroupsService(AppDbContext context) : IGroupsService
{
    public async Task<Result<GroupsListResponse>> GetListAsync(int offset, int take, CancellationToken ctk = default)
    {
        var ordering = new CreatedAtOrderingSpec<Group>();

        var projection = 
            new SelectionSpec<Group>(offset, take)
                .WithProjectionOf(new GroupToSummarySpec());

        var groups = await context.Groups
            .WithSpecification(ordering)
            .WithSpecification(projection)
            .ToListAsync(cancellationToken: ctk);
        
        return Result.Ok(new GroupsListResponse(groups, await context.Groups.CountAsync(ctk)));   
    }

    public async Task<Result<GroupMembersListResponse>> GetMembersListAsync(Guid groupId, int offset, int take, CancellationToken ctk = default)
    {
        var ordering = new CreatedAtOrderingSpec<GroupMember>();
        var groupSpecified = new ByGroupIdSpec(groupId);

        var projection = 
            new SelectionSpec<GroupMember>(offset, take)
                .WithProjectionOf(new GroupMemberToSummarySpec());

        var groups = await context.GroupMembers
            .WithSpecification(groupSpecified)
            .WithSpecification(ordering)
            .WithSpecification(projection)
            .ToListAsync(cancellationToken: ctk);

        var totalCount = await context.GroupMembers
            .WithSpecification(groupSpecified)
            .CountAsync(cancellationToken: ctk);
        
        return Result.Ok(new GroupMembersListResponse(groups, totalCount)); 
    }

    public async Task<Result<GroupSummaryResponse>> CreateAsync(Guid ownerId, CreateGroupRequest request, CancellationToken ctk = default)
    {
        var creation = Group.Create(request.Name);

        if (creation.IsFailed)
            return creation.ToResult();

        var user = await context.Users
            .WithSpecification(new ByIdSpec<User>(ownerId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (user is null)
            return Result.Fail<GroupSummaryResponse>("User not found");

        var group = creation.Value;
        
        var member = group.AddMember(user);
        
        if (member.IsFailed)
            return member.ToResult();
        
        await context.AddAsync(group, ctk);
        await context.SaveChangesAsync(ctk);

        return Result.Ok(new GroupSummaryResponse(group.Id, group.Name, group.Members.Count));
    }

    public async Task<Result<GroupMemberSummaryResponse>> JoinAsync(Guid userId, Guid groupId, CancellationToken ctk = default)
    {
        var group = await context.Groups
            .WithSpecification(new ByIdSpec<Group>(groupId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (group is null)
            return Result.Fail<GroupMemberSummaryResponse>("Group not found");
        
        var user = await context.Users
            .WithSpecification(new ByIdSpec<User>(userId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (user is null)
            return Result.Fail<GroupMemberSummaryResponse>("User not found");
        
        var isMember = await context.GroupMembers.AnyAsync(
            x => x.UserId == user.Id && x.GroupId == groupId, 
            cancellationToken: ctk);
        
        if (isMember)
            return Result.Fail<GroupMemberSummaryResponse>("User is already a member of this group");

        var memberResult = group.AddMember(user);

        if (memberResult.IsFailed)
            return memberResult.ToResult();
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok(new GroupMemberSummaryResponse(
            new UserSummaryResponse(user.Id, user.Nickname, user.CreatedAt), 
            groupId));
    }

    public async Task<Result> LeftAsync(Guid userId, Guid groupId, CancellationToken ctk = default)
    {
        var member = await context.GroupMembers
            .WithSpecification(new ByUserIdSpec(userId))
            .WithSpecification(new ByGroupIdSpec(groupId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (member is null)
            return Result.Fail("Group member not found");
        
        context.GroupMembers.Remove(member);
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(Guid groupId, UpdateGroupRequest request, CancellationToken ctk = default)
    {
        var group = await context.Groups
            .WithSpecification(new ByIdSpec<Group>(groupId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (group is null)
            return Result.Fail("Group not found");
        
        var nameUpdate = group.UpdateName(request.NewName);

        if (nameUpdate.IsFailed)
            return nameUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }
}