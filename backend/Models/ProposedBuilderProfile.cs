namespace ArgosHound.Api.Models;

public sealed class ProposedBuilderProfile
{
    public required string Name { get; set; }

    public List<string> CurrentSkills { get; set; } = [];

    public List<string> LearningGoals { get; set; } = [];

    public List<string> Interests { get; set; } = [];

    public List<string> PreferredOpportunityTypes { get; set; } = [];

    public string? Location { get; set; }

    public List<string> EffortPreferences { get; set; } = [];
}
