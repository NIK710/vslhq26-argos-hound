using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed record AnalyzeDiscussionResponse(
    Guid DiscussionId,
    OpportunityAnalysis Analysis,
    DateTimeOffset AnalyzedAt);
