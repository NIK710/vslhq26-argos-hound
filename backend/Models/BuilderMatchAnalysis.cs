namespace ArgosHound.Api.Models;

public sealed class BuilderMatchAnalysis
{
    public required BuilderOpportunitySubtype Subtype { get; init; }
    public required IReadOnlyList<string> MatchedSkills { get; init; }
    public required IReadOnlyList<string> AdvancedGoals { get; init; }
    public required string EffortEstimate { get; init; }
    public required IReadOnlyList<string> NextSteps { get; init; }
}
