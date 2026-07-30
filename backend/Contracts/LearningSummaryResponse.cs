namespace ArgosHound.Api.Contracts;

public sealed record LearningAggregate(string Value, int Opportunities, int Decisions, int Outcomes);

public sealed record LearningSummaryResponse(
    IReadOnlyList<LearningAggregate> Sources,
    IReadOnlyList<LearningAggregate> Communities,
    IReadOnlyList<LearningAggregate> Topics,
    IReadOnlyList<LearningAggregate> Products,
    IReadOnlyList<LearningAggregate> OpportunityTypes);
