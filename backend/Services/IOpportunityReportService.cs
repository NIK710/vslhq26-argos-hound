using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IOpportunityReportService
{
    Task<OpportunityDetailResponse> BuildAsync(
        Opportunity opportunity,
        CancellationToken cancellationToken = default);
}
