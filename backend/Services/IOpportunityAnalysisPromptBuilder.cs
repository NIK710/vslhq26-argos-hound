using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IOpportunityAnalysisPromptBuilder
{
    Task<string> BuildAsync(
        OpportunityAnalysisContext context,
        CancellationToken cancellationToken = default);
}
