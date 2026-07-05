namespace Tracker.Application.Contracts.Common.Requests;

public sealed record ListSelectionRequest(
    int Take, 
    int Offset
);