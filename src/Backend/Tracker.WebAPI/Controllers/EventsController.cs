using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Application.Contracts.Events.Responses;
using Tracker.Application.Interfaces.Events;
using Tracker.WebAPI.Controllers.Abstractions;

namespace Tracker.WebAPI.Controllers;

[Route("events")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class EventsController(IEventsService eventsService) : TrackerControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EventSummaryResponse>> CreateEvent(CreateEventRequest request)
    {
        var result = await eventsService.CreateAsync(UserId, request);
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<ActionResult<EventsListResponse>> GetEvents(
        [FromQuery] Guid organizerId, [FromQuery] ListSelectionRequest selection)
    {
        var result = await eventsService.GetListAsync(organizerId, selection.Offset, selection.Take);
        return result.ToActionResult();
    }
    
    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<EventsListResponse>> GetEvent(Guid eventId)
    {
        var result = await eventsService.GetAsync(eventId);
        return result.ToActionResult();
    }
    
    [HttpDelete("{eventId:guid}")]
    public async Task<ActionResult<EventsListResponse>> DeleteEvent(Guid eventId)
    {
        var result = await eventsService.DeleteAsync(eventId);
        return result.ToActionResult();
    }
    
    [HttpPut("{eventId:guid}/title")]
    public async Task<ActionResult<EventsListResponse>> UpdateTitle(
        Guid eventId, UpdateEventTitleRequest request)
    {
        var result = await eventsService.UpdateTitleAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("{eventId:guid}/description")]
    public async Task<ActionResult<EventsListResponse>> UpdateDescription(
        Guid eventId, UpdateEventDescriptionRequest request)
    {
        var result = await eventsService.UpdateDescriptionAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("{eventId:guid}/dates")]
    public async Task<ActionResult<EventsListResponse>> UpdateDates(
        Guid eventId, UpdateEventDatesRequest request)
    {
        var result = await eventsService.UpdateDatesAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("{eventId:guid}/location")]
    public async Task<ActionResult<EventsListResponse>> UpdateLocation(
        Guid eventId, UpdateEventLocationRequest request)
    {
        var result = await eventsService.UpdateLocationAsync(eventId, request);
        return result.ToActionResult();
    }
}