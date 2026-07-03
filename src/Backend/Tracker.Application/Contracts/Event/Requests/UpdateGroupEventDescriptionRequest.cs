using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Contracts.Event.Requests;

public sealed record UpdateGroupEventDescriptionRequest(
    FormatTextRequest NewDescription
);