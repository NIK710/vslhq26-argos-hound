using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class OpportunityReportService(
    ISourceDiscussionService sourceDiscussionService)
    : IOpportunityReportService
{
    public OpportunityDetailResponse Build(Opportunity opportunity)
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
            relevantComments);
    }
}
