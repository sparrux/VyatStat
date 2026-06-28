namespace Tracker.Domain.Common;

public abstract class Auditable : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}