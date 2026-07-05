using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Organizers.Requests;
using Tracker.Application.Contracts.Organizers.Responses;
using Tracker.Application.Interfaces;
using Tracker.Application.Interfaces.Organizers;
using Tracker.WebAPI.Controllers.Abstractions;

namespace Tracker.WebAPI.Controllers.Events;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class EventOrganizersController(
    IEventOrganizersService service
) : TrackerControllerBase
{
    [HttpPost("events/{eventId:guid}/organizers")]
    public async Task<ActionResult<EventOrganizerResponse>> CreateOrganizer(
        Guid eventId,
        CreateEventOrganizerRequest request)
    {
        var result = await service.CreateAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpGet("organizers")]
    public async Task<ActionResult<EventOrganizersListResponse>> GetOrganizers(
        [FromQuery] EventOrganizerFilterRequest request,
        [FromQuery] ListSelectionRequest selection)
    {
        var result = await service.GetListAsync(request, selection);
        return result.ToActionResult();
    }
    
    [HttpGet("organizers/{organizerId:guid}")]
    public async Task<ActionResult<EventOrganizerResponse>> GetOrganizer(Guid organizerId)
    {
        var result = await service.GetAsync(organizerId);
        return result.ToActionResult();
    }
    
    [HttpDelete("organizers/{organizerId:guid}")]
    public async Task<ActionResult<EventOrganizerResponse>> DeleteOrganizer(Guid organizerId)
    {
        var result = await service.GetAsync(organizerId);
        return result.ToActionResult();
    }
}