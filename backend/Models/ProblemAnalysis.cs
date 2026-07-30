namespace ArgosHound.Api.Models;

public sealed class ProblemAnalysis
{
    public required string Summary { get; init; }

    public required bool Inferred { get; init; }
}
