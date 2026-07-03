using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Event.Requests;
using Tracker.Application.Contracts.Event.Responses;
using Tracker.Application.Services.Events;

namespace Tracker.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class GroupEventsController(IGroupEventsService eventsService) : ApiControllerBase
{
    [HttpPost("groups/{groupId:guid}/events")]
    public async Task<ActionResult<GroupEventSummaryResponse>> CreateEvent(Guid groupId, CreateGroupEventRequest request)
    {
        var result = await eventsService.CreateAsync(groupId, UserId, request);
        return result.ToActionResult();
    }
    
    [HttpGet("groups/{groupId:guid}/events")]
    public async Task<ActionResult<GroupEventsListResponse>> GetEvents(Guid groupId, [FromQuery] PageSelectionRequest selection)
    {
        var result = await eventsService.GetListAsync(groupId, selection.Offset, selection.Take);
        return result.ToActionResult();
    }
    
    [HttpGet("events/{eventId:guid}")]
    public async Task<ActionResult<GroupEventsListResponse>> GetEvent(Guid eventId)
    {
        var result = await eventsService.GetAsync(eventId);
        return result.ToActionResult();
    }
    
    [HttpDelete("events/{eventId:guid}")]
    public async Task<ActionResult<GroupEventsListResponse>> DeleteEvent(Guid eventId)
    {
        var result = await eventsService.DeleteAsync(eventId);
        return result.ToActionResult();
    }
    
    [HttpPut("events/{eventId:guid}/title")]
    public async Task<ActionResult<GroupEventsListResponse>> UpdateTitle(
        Guid eventId, UpdateGroupEventTitleRequest request)
    {
        var result = await eventsService.UpdateTitleAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("events/{eventId:guid}/description")]
    public async Task<ActionResult<GroupEventsListResponse>> UpdateDescription(
        Guid eventId, UpdateGroupEventDescriptionRequest request)
    {
        var result = await eventsService.UpdateDescriptionAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("events/{eventId:guid}/dates")]
    public async Task<ActionResult<GroupEventsListResponse>> UpdateDates(
        Guid eventId, UpdateGroupEventDatesRequest request)
    {
        var result = await eventsService.UpdateDatesAsync(eventId, request);
        return result.ToActionResult();
    }
    
    [HttpPut("events/{eventId:guid}/location")]
    public async Task<ActionResult<GroupEventsListResponse>> UpdateLocation(
        Guid eventId, UpdateGroupEventLocationRequest request)
    {
        var result = await eventsService.UpdateLocationAsync(eventId, request);
        return result.ToActionResult();
    }
}