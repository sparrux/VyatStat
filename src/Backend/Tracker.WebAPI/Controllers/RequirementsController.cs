using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.GroupEvents.Responses;
using Tracker.Application.Contracts.Requirements.Requests;
using Tracker.Application.Services.Requirements;

namespace Tracker.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class RequirementsController(IRequirementsService service) : ApiControllerBase
{
    [HttpPost("events/{eventId:guid}/requirements")]
    public async Task<ActionResult<GroupEventsListResponse>> CreateRequirement(
        Guid eventId, CreateGroupEventRequirementRequest request)
    {
        var result = await service.CreateAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("events/{eventId:guid}/requirements/{requirementId:guid}")]
    public async Task<ActionResult<GroupEventsListResponse>> CreateRequirement(
        Guid eventId, Guid requirementId, UpdateGroupEventRequirementRequest request)
    {
        var result = await service.UpdateAsync(eventId, requirementId, request);
        return result.ToActionResult();
    }
    
    [HttpDelete("events/{eventId:guid}/requirements/{requirementId:guid}")]
    public async Task<ActionResult<GroupEventsListResponse>> DeleteRequirement(Guid eventId, Guid requirementId)
    {
        var result = await service.DeleteAsync(eventId, requirementId);
        return result.ToActionResult();
    }
}