namespace Tracker.Application.Contracts.Common.Requests;

public sealed record PageSelectionRequest
{
    public int Take { get; init; }
    
    public int Offset { get; init; }
}