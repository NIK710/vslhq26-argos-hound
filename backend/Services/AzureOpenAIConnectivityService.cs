using System.Net.Http.Json;
using ArgosHound.Api.Configuration;
using Microsoft.Extensions.Options;

namespace ArgosHound.Api.Services;

public sealed class AzureOpenAIConnectivityService(
    HttpClient httpClient,
    IOptions<AzureOpenAIOptions> options) : IAzureOpenAIConnectivityService
{
    private readonly AzureOpenAIOptions _options = options.Value;

    public async Task<AzureOpenAIConnectivityResult> CheckAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "responses",
            new
            {
                model = _options.DeploymentName,
                input = "Reply with only OK.",
                max_output_tokens = 16,
                store = false,
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var providerMessage = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Azure OpenAI returned {(int)response.StatusCode}: {providerMessage}",
                inner: null,
                response.StatusCode);
        }

        return new AzureOpenAIConnectivityResult(
            Status: "Connected",
            DeploymentName: _options.DeploymentName);
    }
}
