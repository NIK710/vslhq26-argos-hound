namespace ArgosHound.Api.Models;

public enum OutcomeType
{
    Activation,
    Purchase,
    LearningValue,
    PrototypeCompleted,
    Portfolio,
    Collaboration,
    Interview,
    Contract,
}

public sealed record Outcome(
    Guid Id,
    Guid OpportunityId,
    OutcomeType OutcomeType,
    string? Note,
    DateTimeOffset OccurredAt);
