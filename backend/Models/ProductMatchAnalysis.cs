namespace ArgosHound.Api.Models;

public sealed class ProductMatchAnalysis
{
    public required Guid ProductId { get; init; }

    public required string ProductName { get; init; }

    public required ProductMatchType MatchType { get; init; }

    public required IReadOnlyList<string> MatchedCapabilities { get; init; }
}
