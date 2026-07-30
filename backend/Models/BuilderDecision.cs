namespace ArgosHound.Api.Models;

public enum BuilderDecisionType { Saved, Dismissed, Pursued }

public sealed record BuilderDecision(
    Guid Id,
    Guid OpportunityId,
    BuilderDecisionType DecisionType,
    string? Reason,
    DateTimeOffset OccurredAt);
