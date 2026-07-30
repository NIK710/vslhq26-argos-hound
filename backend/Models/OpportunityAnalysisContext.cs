namespace ArgosHound.Api.Models;

public sealed record OpportunityAnalysisContext(
    BuilderProfile Builder,
    IReadOnlyList<Product> Products,
    SourceDiscussion Discussion);
