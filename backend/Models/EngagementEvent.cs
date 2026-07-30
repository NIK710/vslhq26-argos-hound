namespace ArgosHound.Api.Models;

public sealed class EngagementEvent
{
    public required Guid Id { get; init; }

    public required Guid CampaignLinkId { get; init; }

    public required EngagementEventType EventType { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required IReadOnlyDictionary<string, string> Metadata { get; init; }
}
