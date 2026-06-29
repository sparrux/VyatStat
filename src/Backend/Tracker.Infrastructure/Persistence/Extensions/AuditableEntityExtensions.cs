using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tracker.Domain.Common;

namespace Tracker.Infrastructure.Persistence.Extensions;

static class AuditableEntityExtensions
{
    internal static void UpdateAuditableTimestamps(this ChangeTracker changeTracker, TimeProvider timeProvider)
    {
        var utcNow = timeProvider.GetUtcNow();

        foreach (var entry in changeTracker.Entries<Auditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    break;
            }
        }
    }
}
