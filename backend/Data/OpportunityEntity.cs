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

    public string? BuilderSubtype { get; set; }
    public string MatchedSkillsJson { get; set; } = "[]";
    public string AdvancedGoalsJson { get; set; } = "[]";
    public string? EffortEstimate { get; set; }
    public string NextStepsJson { get; set; } = "[]";

    public required string LimitationsJson { get; set; }

    public required string Explanation { get; set; }

    public required string SuggestedAction { get; set; }

    public decimal Confidence { get; set; }

    public int Score { get; set; }

    public required string ScoreFactorsJson { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<OpportunityEvidenceEntity> EvidenceReferences { get; set; } = [];

    public List<CampaignLinkEntity> CampaignLinks { get; set; } = [];
    public List<BuilderDecisionEntity> Decisions { get; set; } = [];
    public List<OutcomeEntity> Outcomes { get; set; } = [];
}
