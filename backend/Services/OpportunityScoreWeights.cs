namespace ArgosHound.Api.Services;

public static class OpportunityScoreWeights
{
    public const int EvidencePerReference = 5;
    public const int EvidenceMaximum = 20;
    public const int ExplicitProblem = 15;
    public const int InferredProblem = 8;
    public const int DirectProductFit = 35;
    public const int AdjacentProductFit = 25;
    public const int SmallExtensionFit = 18;
    public const int BuilderFitMaximum = 30;
    public const int Actionability = 15;
    public const int LimitationPenalty = 3;
    public const int LimitationPenaltyMaximum = 15;

    public const int SavedDecision = 3;
    public const int DismissedDecision = -6;
    public const int PursuedDecision = 7;
    public const int LearningOutcome = 6;
    public const int CareerOutcome = 9;
    public const int HistoryMaximum = 20;
}
