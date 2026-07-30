using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IOpportunityRepository
{
    Task<IReadOnlyList<Opportunity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Opportunity?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Opportunity?> GetByDiscussionAsync(
        Guid discussionId,
        CancellationToken cancellationToken = default);

    Task<Opportunity> AddAsync(
        Opportunity opportunity,
        CancellationToken cancellationToken = default);
}
