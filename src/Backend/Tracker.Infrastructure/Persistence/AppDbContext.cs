using Microsoft.EntityFrameworkCore;
using Tracker.Domain;
using Tracker.Domain.Events;
using Tracker.Domain.Events.Invitees;
using Tracker.Domain.Events.Requirements;
using Tracker.Domain.Groups;
using Tracker.Domain.Presets;

namespace Tracker.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        Users = Set<User>();
        
        Groups = Set<Group>();
        GroupEvent = Set<GroupEvent>();
        GroupMembers = Set<GroupMember>();
        
        LocationPresets = Set<LocationPreset>();
        RequirementPresets = Set<RequirementPreset>();
        
        Events = Set<Event>();
        EventGoals = Set<EventGoal>();
        EventInvitees = Set<EventInvitee>();
        EventOrganizers = Set<EventOrganizer>();
        EventRequirements = Set<EventRequirement>();
        EventLocations = Set<EventLocation>();
        EventDescriptions = Set<EventDescription>();
        EventRequirementCompletions = Set<EventRequirementCompletion>();
    }


    public DbSet<User> Users { get; }
    
    public DbSet<Group> Groups { get; }
    public DbSet<GroupEvent> GroupEvent { get; }
    public DbSet<GroupMember> GroupMembers { get; }
    
    public DbSet<LocationPreset> LocationPresets { get; }
    public DbSet<RequirementPreset> RequirementPresets { get; }
    
    public DbSet<Event> Events { get; }
    public DbSet<EventGoal> EventGoals { get; }
    public DbSet<EventInvitee> EventInvitees { get; }
    public DbSet<EventLocation> EventLocations { get; }
    public DbSet<EventOrganizer> EventOrganizers { get; }
    public DbSet<EventDescription> EventDescriptions { get; }
    public DbSet<EventRequirement> EventRequirements { get; }
    public DbSet<EventRequirementCompletion> EventRequirementCompletions { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
