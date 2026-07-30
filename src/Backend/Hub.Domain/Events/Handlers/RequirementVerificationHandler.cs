using Ardalis.Result;
using Hub.Domain.Events.Participants;
using Hub.Domain.Events.Requirements;
using Hub.Domain.Extensions;

namespace Hub.Domain.Events.Handlers;

abstract record Verification(
    Guid ParticipantUser,
    Guid Requirement
);

record VerifyByActor(
    Guid ParticipantUser,
    Guid Requirement,
    Guid Actor
) : Verification(ParticipantUser, Requirement);

sealed record VerifyByAutomatic(
    Guid ParticipantUser,
    Guid Requirement
) : Verification(ParticipantUser, Requirement);

sealed class RequirementVerificationHandler(Event evt)
{
    public Result SubmitVerification(Verification verification)
    {
        throw new NotImplementedException();
        
        // var participant = evt.Participants.FindParticipant(verification.ParticipantUser);
        // if (participant is null)
        //     return Result.NotFound("Participant not found");
        //
        // var requirement = evt.Requirements.FindRequirement(verification.Requirement);
        // if (requirement is null)
        //     return Result.NotFound("Event Requirement not found");
        //
        // var assignment = participant.Requirements.FindAssignment(verification.Requirement);
        // if (assignment is null)
        //     return Result.NotFound("Event Assignment not found");
        //
        // return verification switch
        // {
        //     VerifyByActor byActor => VerifyManualByActor(requirement, assignment, byActor.Actor),
        //     VerifyByAutomatic _ => VerifyByAutomatic(requirement, assignment),
        //     _ => throw new DomainException("Unknown verification case")
        // };
    }

    // Result VerifyManualByActor(
    //     EventRequirement requirement,
    //     EventRequirementAssignment completion,
    //     Guid actor)
    // {
    //     if (!requirement.IsManualByUserMode() && !requirement.IsManualByOrganizerMode())
    //         return Result.Forbidden("Verification mode is not manual by actor");
    //     
    //     if (!evt.IsParticipant(actor))
    //         return Result.Forbidden("Verification actor is not an event participant");
    //
    //     if (evt.IsInvitee(actor) && requirement.IsManualByUserMode() && completion.Owns(actor))
    //         return completion.Verify();
    //     
    //     if (evt.IsInvitee(actor) && requirement.IsManualByOrganizerMode() && completion.Owns(actor))
    //         return completion.PendingVerification();
    //     
    //     if (evt.IsOrganizer(actor))
    //         return completion.Verify();
    //     
    //     return Result.Forbidden("Something went wrong");
    // }
    //
    // static Result VerifyByAutomatic(
    //     EventRequirement requirement, 
    //     EventRequirementAssignment completion) =>
    //     !requirement.IsAutomaticMode() 
    //         ? Result.Forbidden("Verification mode is not automatic") 
    //         : completion.Verify();
}

file static class VerificationHelper
{
    public static EventParticipant? FindParticipant(
        this IEnumerable<EventParticipant> participants, Guid userId) =>
        participants.FirstOrDefault(i => i.UserId == userId);
    
    public static EventRequirement? FindRequirement(
        this IEnumerable<EventRequirement> requirements, Guid requirement) =>
        requirements.FirstOrDefault(i => i.Id == requirement);

    public static EventRequirementAssignment? FindAssignment(
        this IEnumerable<EventRequirementAssignment> completions, Guid requirement) =>
        completions.FirstOrDefault(i => i.RequirementId == requirement);

    public static bool Owns(
        this EventRequirementAssignment completion, Guid actor) =>
        completion.AssignParticipant.UserId == actor;
    
    extension(Event evt)
    {
        public bool IsInvitee(Guid actor) =>
            evt.Participants.Any(x => x.UserId == actor);

        public bool IsParticipant(Guid actor) =>
            evt.IsInvitee(actor) || evt.IsOrganizer(actor);
    }
}