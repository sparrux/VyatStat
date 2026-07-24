using Hub.Domain;
using Hub.Domain.Events;
using Hub.Domain.Events.Goals;
using Hub.Domain.Events.Participants;
using Hub.Domain.Events.Requirements;
using Hub.Domain.Groups;
using Hub.Domain.Groups.Training;
using Microsoft.EntityFrameworkCore;

namespace Hub.Infrastructure.Persistence;

public sealed class HubDbContext : DbContext
{
    public HubDbContext(DbContextOptions options) : base(options)
    {
        Users = Set<User>();
        
        TrainingModules = Set<TrainingModule>();
        TrainingModuleRatings = Set<TrainingRating>();
        
        Groups = Set<Group>();
        GroupEvent = Set<GroupEvent>();
        GroupMembers = Set<GroupMember>();
        
        Events = Set<Event>();
        EventRoles = Set<EventRole>();
        EventGoals = Set<EventGoal>();
        EventGoalsTasks = Set<EventGoalTask>();
        EventParticipants = Set<EventParticipant>();
        EventParticipantRoles = Set<EventParticipantRole>();
        EventRequirements = Set<EventRequirement>();
        EventLocations = Set<EventLocation>();
        EventRequirementAssignments = Set<EventRequirementAssignment>();
    }
    
    public DbSet<User> Users { get; }
    
    public DbSet<TrainingModule> TrainingModules { get; }
    public DbSet<TrainingRating> TrainingModuleRatings { get; }
    
    public DbSet<Group> Groups { get; }
    public DbSet<GroupEvent> GroupEvent { get; }
    public DbSet<GroupMember> GroupMembers { get; }
    
    public DbSet<Event> Events { get; }
    public DbSet<EventRole> EventRoles { get; }
    public DbSet<EventGoal> EventGoals { get; }
    public DbSet<EventLocation> EventLocations { get; }
    public DbSet<EventRequirement> EventRequirements { get; }
    public DbSet<EventGoalTask> EventGoalsTasks { get; }
    public DbSet<EventParticipant> EventParticipants { get; }
    public DbSet<EventParticipantRole> EventParticipantRoles { get; }
    public DbSet<EventRequirementAssignment> EventRequirementAssignments { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HubDbContext).Assembly);
    }
}