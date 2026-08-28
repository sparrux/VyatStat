using Hub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Hub.Infrastructure.Persistence.Interceptors;

public sealed class AuditInterceptor(
    TimeProvider timeProvider
) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            throw new InvalidOperationException($"{nameof(eventData.Context)} cannot be null.");
        
        UpdateEntries(eventData.Context);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    void UpdateEntries(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Entity is Auditable auditable)
                UpdateAuditable(entry, auditable);
        }
    }

    void UpdateAuditable(EntityEntry entry, Auditable auditable)
    {
        if (entry.State is EntityState.Added)
        {
            auditable.CreatedAt = timeProvider.GetUtcNow();
            auditable.UpdatedAt = timeProvider.GetUtcNow();
        }

        if (entry.State is EntityState.Modified) auditable.UpdatedAt = timeProvider.GetUtcNow();
    }
}