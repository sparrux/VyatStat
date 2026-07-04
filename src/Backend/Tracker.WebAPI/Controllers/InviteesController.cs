using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Services.Invitees;

namespace Tracker.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class InviteesController(IInviteesService inviteesService) : ApiControllerBase
{
    [HttpPost("events/{eventId:guid}/invitees")]
    public async Task<ActionResult<GroupEventInviteeSummaryResponse>> CreateInvitee(
        Guid eventId, [FromQuery] Guid userId)
    {
        var result = await inviteesService.CreateAsync(eventId, userId);
        return result.ToActionResult();
    }
    
    [HttpGet("events/{eventId:guid}/invitees")]
    public async Task<ActionResult<GroupEventInviteesListResponse>> GetInvitees(
        Guid eventId, [FromQuery] PageSelectionRequest selection)
    {
        var result = await inviteesService.GetListAsync(eventId, selection.Offset, selection.Take);
        return result.ToActionResult();
    }
    
    [HttpGet("events/{eventId:guid}/invitees/{userId:guid}")]
    public async Task<ActionResult<GroupEventInviteeDetailsResponse>> GetInvitee(
        Guid eventId, Guid userId)
    {
        var result = await inviteesService.GetAsync(eventId, userId);
        return result.ToActionResult();
    }
}