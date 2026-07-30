namespace ArgosHound.Api.Services;

public interface IFoundryAgentConnectivityService
{
    Task<FoundryAgentConnectivityResult> CheckAsync(
        CancellationToken cancellationToken = default);
}
