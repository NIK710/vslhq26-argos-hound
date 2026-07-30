namespace ArgosHound.Api.Configuration;

public sealed class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    public required string Endpoint { get; init; }

    public required string DeploymentName { get; init; }

    public required string ApiKey { get; init; }
}
