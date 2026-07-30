using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed record RecordDecisionRequest(BuilderDecisionType DecisionType, string? Reason);
public sealed record RecordOutcomeRequest(OutcomeType OutcomeType, string? Note);
public sealed record OpportunityActivityResponse(
    IReadOnlyList<BuilderDecision> Decisions,
    IReadOnlyList<Outcome> Outcomes);
