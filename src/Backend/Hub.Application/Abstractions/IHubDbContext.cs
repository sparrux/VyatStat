using Hub.Domain;
using Hub.Domain.Events;
using Hub.Domain.Events.Goals;
using Hub.Domain.Events.Participants;
using Hub.Domain.Events.Requirements;
using Hub.Domain.Groups;
using Hub.Domain.Groups.Members;
using Hub.Domain.Groups.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hub.Application.Abstractions;

public interface IHubDbContext
{
    DbSet<User> Users { get; }

    DbSet<TrainingModule> TrainingModules { get; }
    DbSet<TrainingRating> TrainingModuleRatings { get; }

    DbSet<Group> Groups { get; }
    DbSet<GroupRole> GroupRoles { get; }
    DbSet<GroupEvent> GroupEvent { get; }
    DbSet<GroupMember> GroupMembers { get; }

    DbSet<Event> Events { get; }
    DbSet<EventRole> EventRoles { get; }
    DbSet<EventGoal> EventGoals { get; }
    DbSet<EventLocation> EventLocations { get; }
    DbSet<EventRequirement> EventRequirements { get; }
    DbSet<EventGoalTask> EventGoalsTasks { get; }
    DbSet<EventParticipant> EventParticipants { get; }
    DbSet<EventParticipantRole> EventParticipantRoles { get; }
    DbSet<EventRequirementAssignment> EventRequirementAssignments { get; }
    DbSet<EventRequirementVerifier> EventRequirementVerifiers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
