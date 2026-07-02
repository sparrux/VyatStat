using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Group.Requests;
using Tracker.Application.Contracts.Group.Responses;
using Tracker.Application.Contracts.GroupMember.Responses;
using Tracker.Application.Services.Groups;

namespace Tracker.WebAPI.Controllers;

[Route("groups")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class GroupsController(IGroupsService groupsService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<GroupSummaryResponse>> CreateGroup(CreateGroupRequest request)
    {
        var result = await groupsService.CreateAsync(UserId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("{groupId:guid}")]
    public async Task<IActionResult> UpdateGroup(Guid groupId, UpdateGroupRequest request)
    {
        var result = await groupsService.UpdateAsync(groupId, request);
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<ActionResult<GroupsListResponse>> GetGroups(int offset, int take)
    {
        var result = await groupsService.GetListAsync(offset, take);
        return result.ToActionResult();
    }

    [HttpGet("{groupId:guid}/members")]
    public async Task<ActionResult<GroupMembersListResponse>> GetMembers(Guid groupId, int offset, int take)
    {
        var result = await groupsService.GetMembersListAsync(groupId, offset, take);
        return result.ToActionResult();
    }
    
    [HttpPost("{groupId:guid}/members")]
    public async Task<ActionResult<GroupMemberSummaryResponse>> Join(Guid groupId)
    {
        var result = await groupsService.JoinAsync(UserId, groupId);
        return result.ToActionResult();
    }
    
    [HttpDelete("{groupId:guid}/members")]
    public async Task<ActionResult<GroupMemberSummaryResponse>> Left(Guid groupId)
    {
        var result = await groupsService.LeftAsync(UserId, groupId);
        return result.ToActionResult();
    }
}