using ArgosHound.Api.Configuration;
using ArgosHound.Api.Models;
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Options;
using System.ClientModel;
#pragma warning disable OPENAI001
using OpenAI.Responses;
#pragma warning restore OPENAI001

namespace ArgosHound.Api.Services;

public sealed class FoundryLlmAnalysisProvider(
    IOptions<FoundryOptions> options,
    IOpportunityAnalysisPromptBuilder promptBuilder,
    OpportunityAnalysisValidator validator,
    ILogger<FoundryLlmAnalysisProvider> logger) : ILlmAnalysisProvider
{
    private readonly FoundryOptions _options = options.Value;

    public async Task<OpportunityAnalysis> AnalyzeAsync(
        OpportunityAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var prompt = await promptBuilder.BuildAsync(context, cancellationToken);

        for (var attempt = 1; attempt <= _options.MaxAttempts; attempt++)
        {
            try
            {
                var output = await InvokeAgentAsync(
                    prompt,
                    cancellationToken);
                try
                {
                    return validator.ParseAndValidate(output, context);
                }
                catch (LlmAnalysisOutputException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Foundry returned rejected analysis output.");
                    throw;
                }
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                if (attempt == _options.MaxAttempts)
                {
                    throw new LlmAnalysisTimeoutException(
                        "The Foundry agent did not respond before the analysis timeout.",
                        exception);
                }

                logger.LogWarning(
                    "Foundry analysis attempt {Attempt} timed out; retrying.",
                    attempt);
            }
            catch (ClientResultException exception)
                when (IsTransient(exception.Status)
                    && attempt < _options.MaxAttempts)
            {
                logger.LogWarning(
                    exception,
                    "Foundry analysis attempt {Attempt} failed transiently; retrying.",
                    attempt);
            }
            catch (ClientResultException exception)
            {
                throw new LlmAnalysisUnavailableException(
                    "The Foundry agent is temporarily unavailable.",
                    exception);
            }
            catch (AuthenticationFailedException exception)
            {
                throw new LlmAnalysisUnavailableException(
                    "The backend could not authenticate with Foundry.",
                    exception);
            }
            catch (AggregateException exception)
            {
                throw new LlmAnalysisUnavailableException(
                    "The backend could not connect to Foundry.",
                    exception);
            }

            await Task.Delay(
                TimeSpan.FromMilliseconds(300 * attempt),
                cancellationToken);
        }

        throw new LlmAnalysisUnavailableException(
            "The Foundry agent is temporarily unavailable.");
    }

    private async Task<string> InvokeAgentAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        using var timeoutSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(
            TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

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
            StoredOutputEnabled = false,
            MaxOutputTokenCount = 2_000,
        };
        responseOptions.InputItems.Add(
            ResponseItem.CreateUserMessageItem(prompt));

        var response = await responsesClient.CreateResponseAsync(
            responseOptions,
            timeoutSource.Token);
        var output = response.Value.GetOutputText();
#pragma warning restore OPENAI001

        if (string.IsNullOrWhiteSpace(output))
        {
            throw new LlmAnalysisOutputException(
                "The model returned an empty structured analysis.");
        }

        return output.Trim();
    }

    private static bool IsTransient(int status) =>
        status is 408 or 429 or >= 500;
}
