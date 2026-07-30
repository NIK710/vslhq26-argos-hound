using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class OpportunityReportService(
    ISourceDiscussionService sourceDiscussionService,
    ICampaignRepository campaignRepository,
    OpportunityActivityService activityService)
    : IOpportunityReportService
{
    public async Task<OpportunityDetailResponse> BuildAsync(
        Opportunity opportunity,
        CancellationToken cancellationToken = default)
    {
        var source = sourceDiscussionService.Get(opportunity.DiscussionId);
        var evidenceIds = opportunity.EvidenceReferences.ToHashSet(
            StringComparer.Ordinal);
        var relevantComments = source.Comments
            .Where(comment => evidenceIds.Contains(comment.ExternalId))
            .ToArray();

        return new OpportunityDetailResponse(
            opportunity,
            source,
            relevantComments,
            await campaignRepository.GetForOpportunityAsync(
                opportunity.Id,
                cancellationToken),
            (await activityService.GetAsync(opportunity.Id, cancellationToken))!);
    }
}
