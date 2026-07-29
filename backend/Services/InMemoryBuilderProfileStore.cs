using ArgosHound.Api.Data;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class InMemoryBuilderProfileStore : IBuilderProfileStore
{
    private readonly object _gate = new();
    private BuilderProfile _profile = Clone(DemoData.Builder);

    public BuilderProfile Get()
    {
        lock (_gate)
        {
            return Clone(_profile);
        }
    }

    public BuilderProfile ReplaceWith(ProposedBuilderProfile proposedProfile)
    {
        lock (_gate)
        {
            _profile = new BuilderProfile
            {
                Id = _profile.Id,
                Name = proposedProfile.Name,
                CurrentSkills = [.. proposedProfile.CurrentSkills],
                LearningGoals = [.. proposedProfile.LearningGoals],
                Interests = [.. proposedProfile.Interests],
                PreferredOpportunityTypes = [.. proposedProfile.PreferredOpportunityTypes],
                Location = proposedProfile.Location,
                EffortPreferences = [.. proposedProfile.EffortPreferences],
                CreatedAt = _profile.CreatedAt,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            return Clone(_profile);
        }
    }

    private static BuilderProfile Clone(BuilderProfile source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            CurrentSkills = [.. source.CurrentSkills],
            LearningGoals = [.. source.LearningGoals],
            Interests = [.. source.Interests],
            PreferredOpportunityTypes = [.. source.PreferredOpportunityTypes],
            Location = source.Location,
            EffortPreferences = [.. source.EffortPreferences],
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
        };
}
