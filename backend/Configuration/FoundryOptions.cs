namespace ArgosHound.Api.Configuration;

public sealed class FoundryOptions
{
    public const string SectionName = "Foundry";

    public string ProjectEndpoint { get; init; } = string.Empty;

    public string AgentName { get; init; } = string.Empty;

    public string AgentVersion { get; init; } = string.Empty;
}
