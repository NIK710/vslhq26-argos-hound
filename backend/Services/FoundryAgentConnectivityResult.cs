namespace ArgosHound.Api.Services;

public sealed record FoundryAgentConnectivityResult(
    string Status,
    string AgentName,
    string AgentVersion);
