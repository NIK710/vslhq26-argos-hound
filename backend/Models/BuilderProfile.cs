namespace ArgosHound.Api.Models;

public sealed class BuilderProfile
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name { get; set; }

    public List<string> CurrentSkills { get; set; } = [];

    public List<string> LearningGoals { get; set; } = [];

    public List<string> Interests { get; set; } = [];

    public List<string> PreferredOpportunityTypes { get; set; } = [];

    public string? Location { get; set; }

    public List<string> EffortPreferences { get; set; } = [];

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
