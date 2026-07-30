namespace ArgosHound.Api.Data;

public sealed class OpportunityEvidenceEntity
{
    public Guid Id { get; set; }

    public Guid OpportunityId { get; set; }

    public required string ExternalId { get; set; }

    public OpportunityEntity? Opportunity { get; set; }
}
