using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Interfaces.Invitees;
using Tracker.WebAPI.Controllers.Abstractions;

namespace Tracker.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Route("events/{eventId:guid}/invitees")]
public sealed class InviteesController(IInviteesService inviteesService) : TrackerControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EventInviteeSummaryResponse>> CreateInvitee(
        Guid eventId, [FromQuery] Guid userId)
    {
        var result = await inviteesService.CreateAsync(eventId, userId);
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<ActionResult<EventInviteesListResponse>> GetInvitees(
        Guid eventId, [FromQuery] ListSelectionRequest selection)
    {
        var result = await inviteesService.GetListAsync(eventId, selection.Offset, selection.Take);
        return result.ToActionResult();
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<EventInviteeDetailsResponse>> GetInvitee(
        Guid eventId, Guid userId)
    {
        var result = await inviteesService.GetAsync(eventId, userId);
        return result.ToActionResult();
    }
}