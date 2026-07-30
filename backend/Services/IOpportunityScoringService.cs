using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IOpportunityScoringService
{
    OpportunityScore Calculate(
        OpportunityAnalysis analysis,
        OpportunityAnalysisContext context);
}
