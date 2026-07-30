namespace ArgosHound.Api.Models;

public sealed record OpportunityScoreFactor(
    string Key,
    string Label,
    int Points,
    string Explanation);
