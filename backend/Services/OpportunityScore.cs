using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed record OpportunityScore(
    int Value,
    IReadOnlyList<OpportunityScoreFactor> Factors);
