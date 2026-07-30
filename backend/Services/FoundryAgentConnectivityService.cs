using ArgosHound.Api.Configuration;
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Options;
#pragma warning disable OPENAI001
using OpenAI.Responses;
#pragma warning restore OPENAI001

namespace ArgosHound.Api.Services;

public sealed class FoundryAgentConnectivityService(
    IOptions<FoundryOptions> options) : IFoundryAgentConnectivityService
{
    private readonly FoundryOptions _options = options.Value;

    public async Task<FoundryAgentConnectivityResult> CheckAsync(
        CancellationToken cancellationToken = default)
    {
        var projectClient = new AIProjectClient(
            new Uri(_options.ProjectEndpoint),
            new DefaultAzureCredential());

        var agentReference = new AgentReference(
            name: _options.AgentName,
            version: _options.AgentVersion);
        var responsesClient = projectClient.ProjectOpenAIClient
            .GetProjectResponsesClientForAgent(agentReference);

#pragma warning disable OPENAI001
        var responseOptions = new CreateResponseOptions
        {
            StoredOutputEnabled = false
        };
        responseOptions.InputItems.Add(
            ResponseItem.CreateUserMessageItem(
                "Connectivity check. Reply with only AGENT_OK."));

        var response = await responsesClient.CreateResponseAsync(
            responseOptions,
            cancellationToken);
        var output = response.Value.GetOutputText().Trim();
#pragma warning restore OPENAI001

        if (!string.Equals(output, "AGENT_OK", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The Foundry agent responded, but not with the expected connectivity token.");
        }

        return new FoundryAgentConnectivityResult(
            "Connected",
            _options.AgentName,
            _options.AgentVersion);
    }
}
