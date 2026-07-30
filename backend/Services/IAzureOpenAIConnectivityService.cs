namespace ArgosHound.Api.Services;

public interface IAzureOpenAIConnectivityService
{
    Task<AzureOpenAIConnectivityResult> CheckAsync(
        CancellationToken cancellationToken = default);
}
