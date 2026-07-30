namespace ArgosHound.Api.Data;

public sealed class BuilderDecisionEntity
{
    public Guid Id { get; set; }
    public Guid OpportunityId { get; set; }
    public required string DecisionType { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public OpportunityEntity Opportunity { get; set; } = null!;
}
