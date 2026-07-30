using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed record OpportunitySummaryResponse(
    Guid Id,
    Guid DiscussionId,
    OpportunityType Type,
    string Problem,
    bool ProblemInferred,
    string Topic,
    int Score,
    decimal Confidence,
    string SuggestedAction,
    DateTimeOffset CreatedAt);
