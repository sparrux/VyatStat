using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Contracts.GroupEvents.Requests;

public sealed record UpdateGroupEventDescriptionRequest(
    FormatTextRequest NewDescription
);