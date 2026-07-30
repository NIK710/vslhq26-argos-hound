namespace ArgosHound.Api.Data;

public sealed class OutcomeEntity
{
    public Guid Id { get; set; }
    public Guid OpportunityId { get; set; }
    public required string OutcomeType { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public OpportunityEntity Opportunity { get; set; } = null!;
}
