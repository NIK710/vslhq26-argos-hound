using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface ICampaignRepository
{
    Task<CampaignLink> AddAsync(
        CampaignLink campaign,
        string codeHash,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignLink>> GetForOpportunityAsync(
        Guid opportunityId,
        CancellationToken cancellationToken = default);

    Task<CampaignRedirectResult> RecordOpenedAsync(
        string codeHash,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default);
}
