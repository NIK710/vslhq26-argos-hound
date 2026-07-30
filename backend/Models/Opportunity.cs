namespace ArgosHound.Api.Models;

public sealed class Opportunity
{
    public required Guid Id { get; init; }

    public required Guid DiscussionId { get; init; }

    public required OpportunityType Type { get; init; }

    public ProductMatchType? ProductMatchType { get; init; }

    public required string Problem { get; init; }

    public required bool ProblemInferred { get; init; }

    public required string Topic { get; init; }

    public required DiscussionSentiment Sentiment { get; init; }

    public Guid? MatchedProductId { get; init; }

    public string? MatchedProductName { get; init; }

    public required IReadOnlyList<string> MatchedCapabilities { get; init; }

    public BuilderOpportunitySubtype? BuilderSubtype { get; init; }

    public IReadOnlyList<string> MatchedSkills { get; init; } = [];

    public IReadOnlyList<string> AdvancedGoals { get; init; } = [];

    public string? EffortEstimate { get; init; }

    public IReadOnlyList<string> NextSteps { get; init; } = [];

    public required IReadOnlyList<string> Limitations { get; init; }

    public required IReadOnlyList<string> EvidenceReferences { get; init; }

    public required string Explanation { get; init; }

    public required string SuggestedAction { get; init; }

    public required decimal Confidence { get; init; }

    public required int Score { get; init; }

    public required IReadOnlyList<OpportunityScoreFactor> ScoreFactors { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}
