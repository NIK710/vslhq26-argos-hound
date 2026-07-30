namespace ArgosHound.Api.Services;

public sealed record AzureOpenAIConnectivityResult(
    string Status,
    string DeploymentName);
