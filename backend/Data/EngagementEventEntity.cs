namespace ArgosHound.Api.Data;

public sealed class EngagementEventEntity
{
    public Guid Id { get; set; }

    public Guid CampaignLinkId { get; set; }

    public required string EventType { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public required string MetadataJson { get; set; }

    public CampaignLinkEntity? CampaignLink { get; set; }
}
