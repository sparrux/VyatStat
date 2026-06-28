using Microsoft.EntityFrameworkCore;
using Tracker.Domain;
using Tracker.Domain.GroupEvents.Events;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        Users = Set<User>();
        Locations = Set<Location>();
        Groups = Set<Group>();
        GroupMembers = Set<GroupMember>();
        GroupEvents = Set<GroupEvent>();
        GroupEventTargets = Set<GroupEventTarget>();
        GroupEventInvitees = Set<GroupEventInvitee>();
        GroupEventOrganizers = Set<GroupEventOrganizer>();
        GroupEventRequirements = Set<GroupEventRequirement>();
        GroupEventLocations = Set<GroupEventLocation>();
        GroupEventDescriptions = Set<GroupEventDescription>();
        GroupEventInviteeRequirementCompletions = Set<GroupEventInviteeRequirementCompletion>();
    }

    public DbSet<User> Users { get; }
    public DbSet<Location> Locations { get; }
    public DbSet<Group> Groups { get; }
    public DbSet<GroupMember> GroupMembers { get; }
    public DbSet<GroupEvent> GroupEvents { get; }
    public DbSet<GroupEventTarget> GroupEventTargets { get; }
    public DbSet<GroupEventInvitee> GroupEventInvitees { get; }
    public DbSet<GroupEventOrganizer> GroupEventOrganizers { get; }
    public DbSet<GroupEventRequirement> GroupEventRequirements { get; }
    public DbSet<GroupEventLocation> GroupEventLocations { get; }
    public DbSet<GroupEventDescription> GroupEventDescriptions { get; }
    public DbSet<GroupEventInviteeRequirementCompletion> GroupEventInviteeRequirementCompletions { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
