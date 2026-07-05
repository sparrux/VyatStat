using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Events.Responses;
using Tracker.Application.Contracts.Requirements.Requests;
using Tracker.Application.Interfaces.Requirements;
using Tracker.WebAPI.Controllers.Abstractions;

namespace Tracker.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Route("events/{eventId:guid}/requirements")]
public sealed class RequirementsController(IRequirementsService service) : TrackerControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EventsListResponse>> CreateRequirement(
        Guid eventId, CreateEventRequirementRequest request)
    {
        var result = await service.CreateAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("{requirementId:guid}")]
    public async Task<ActionResult<EventsListResponse>> CreateRequirement(
        Guid eventId, Guid requirementId, UpdateEventRequirementRequest request)
    {
        var result = await service.UpdateAsync(eventId, requirementId, request);
        return result.ToActionResult();
    }
    
    [HttpDelete("{requirementId:guid}")]
    public async Task<ActionResult<EventsListResponse>> DeleteRequirement(Guid eventId, Guid requirementId)
    {
        var result = await service.DeleteAsync(eventId, requirementId);
        return result.ToActionResult();
    }
}