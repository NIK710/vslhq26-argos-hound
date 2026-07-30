namespace ArgosHound.Api.Models;

public sealed class OpportunityAnalysis
{
    public required ProblemAnalysis Problem { get; init; }

    public required string Topic { get; init; }

    public required DiscussionSentiment Sentiment { get; init; }

    public required IReadOnlyList<string> EvidenceReferences { get; init; }

    public required OpportunityType OpportunityType { get; init; }

    public ProductMatchAnalysis? ProductMatch { get; init; }

    public required IReadOnlyList<string> Limitations { get; init; }

    public required string Explanation { get; init; }

    public required string SuggestedAction { get; init; }

    public required decimal Confidence { get; init; }
}
