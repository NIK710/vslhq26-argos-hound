namespace ArgosHound.Api.Data;

public sealed class OpportunityEntity
{
    public Guid Id { get; set; }

    public Guid DiscussionId { get; set; }

    public required string Type { get; set; }

    public string? ProductMatchType { get; set; }

    public required string Problem { get; set; }

    public bool ProblemInferred { get; set; }

    public required string Topic { get; set; }

    public required string Sentiment { get; set; }

    public Guid? MatchedProductId { get; set; }

    public string? MatchedProductName { get; set; }

    public required string MatchedCapabilitiesJson { get; set; }

    public required string LimitationsJson { get; set; }

    public required string Explanation { get; set; }

    public required string SuggestedAction { get; set; }

    public decimal Confidence { get; set; }

    public int Score { get; set; }

    public required string ScoreFactorsJson { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<OpportunityEvidenceEntity> EvidenceReferences { get; set; } = [];
}
