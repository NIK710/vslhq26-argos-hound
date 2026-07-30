using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IDiscoveryService
{
    Task<Opportunity> DiscoverAsync(
        Guid discussionId,
        CancellationToken cancellationToken = default);
}
