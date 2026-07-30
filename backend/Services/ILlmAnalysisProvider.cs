using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface ILlmAnalysisProvider
{
    Task<OpportunityAnalysis> AnalyzeAsync(
        OpportunityAnalysisContext context,
        CancellationToken cancellationToken = default);
}
