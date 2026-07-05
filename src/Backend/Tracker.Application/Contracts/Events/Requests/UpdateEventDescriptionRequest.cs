using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Contracts.Events.Requests;

public sealed record UpdateEventDescriptionRequest(
    EventDescriptionRequest NewDescription
);