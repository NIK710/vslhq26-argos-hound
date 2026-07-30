namespace ArgosHound.Api.Models;

public sealed class CampaignLink
{
    public required Guid Id { get; init; }

    public required Guid OpportunityId { get; init; }

    public required string DestinationUrl { get; init; }

    public required CampaignPurpose Purpose { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }

    public required IReadOnlyList<EngagementEvent> Events { get; init; }
}
