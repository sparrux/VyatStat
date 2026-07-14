using Ardalis.Result;
using Hub.Domain.Common.Exceptions;
using Hub.Domain.Events.Invitees;
using Hub.Domain.Events.Requirements;

namespace Hub.Domain.Events.Handlers;

abstract record Verification(
    Guid InviteeUser,
    Guid Requirement
);

record VerifyByActor(
    Guid InviteeUser,
    Guid Requirement,
    Guid Actor
) : Verification(InviteeUser, Requirement);

sealed record VerifyByAutomatic(
    Guid InviteeUser,
    Guid Requirement
) : Verification(InviteeUser, Requirement);

sealed class RequirementVerificationHandler(Event evt)
{
    public Result SubmitVerification(Verification verification)
    {
        var invitee = evt.Invitees.FindInvitee(verification.InviteeUser);
        if (invitee is null)
            return Result.NotFound("Invitee not found");
        
        var requirement = evt.Requirements.FindRequirement(verification.Requirement);
        if (requirement is null)
            return Result.NotFound("Event requirement not found");

        var completion = invitee.RequirementCompletions.FindCompletion(verification.Requirement);
        if (completion is null)
            return Result.NotFound("Event completion not found");

        return verification switch
        {
            VerifyByActor byActor => VerifyManualByActor(requirement, completion, byActor.Actor),
            VerifyByAutomatic _ => VerifyByAutomatic(requirement, completion),
            _ => throw new DomainException("Unknown verification case")
        };
    }

    Result VerifyManualByActor(
        EventRequirement requirement,
        EventRequirementCompletion completion,
        Guid actor)
    {
        if (!requirement.IsManualByUserMode() && !requirement.IsManualByOrganizerMode())
            return Result.Forbidden("Verification mode is not manual by actor");
        
        if (!evt.IsParticipant(actor))
            return Result.Forbidden("Verification actor is not an event participant");

        if (evt.IsInvitee(actor) && requirement.IsManualByUserMode() && completion.Owns(actor))
            return completion.Verify();
        
        if (evt.IsInvitee(actor) && requirement.IsManualByOrganizerMode() && completion.Owns(actor))
            return completion.PendingVerification();
        
        if (evt.IsOrganizer(actor))
            return completion.Verify();
        
        return Result.Forbidden("Something went wrong");
    }
    
    static Result VerifyByAutomatic(
        EventRequirement requirement, 
        EventRequirementCompletion completion) =>
        !requirement.IsAutomaticMode() 
            ? Result.Forbidden("Verification mode is not automatic") 
            : completion.Verify();
}

file static class VerificationHelper
{
    public static EventInvitee? FindInvitee(
        this IEnumerable<EventInvitee> invitees, Guid userId) =>
        invitees.FirstOrDefault(i => i.UserId == userId);
    
    public static EventRequirement? FindRequirement(
        this IEnumerable<EventRequirement> requirements, Guid requirement) =>
        requirements.FirstOrDefault(i => i.Id == requirement);

    public static EventRequirementCompletion? FindCompletion(
        this IEnumerable<EventRequirementCompletion> completions, Guid requirement) =>
        completions.FirstOrDefault(i => i.RequirementId == requirement);

    public static bool Owns(
        this EventRequirementCompletion completion, Guid actor) =>
        completion.Invitee.UserId == actor;
    
    extension(Event evt)
    {
        public bool IsInvitee(Guid actor) =>
            evt.Invitees.Any(x => x.UserId == actor);

        public bool IsOrganizer(Guid actor) =>
            evt.Organizers.Any(x => x.UserId == actor);

        public bool IsParticipant(Guid actor) =>
            evt.IsInvitee(actor) || evt.IsOrganizer(actor);
    }
}