namespace ArgosHound.Api.Data;

public sealed class CampaignLinkEntity
{
    public Guid Id { get; set; }

    public Guid OpportunityId { get; set; }

    public required string CodeHash { get; set; }

    public required string DestinationUrl { get; set; }

    public required string Purpose { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public OpportunityEntity? Opportunity { get; set; }

    public List<EngagementEventEntity> Events { get; set; } = [];
}
