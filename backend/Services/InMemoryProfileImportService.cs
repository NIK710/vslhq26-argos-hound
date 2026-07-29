using System.Collections.Concurrent;
using System.Text.Json;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class InMemoryProfileImportService(
    IBuilderProfileStore builderProfileStore) : IProfileImportService
{
    private const int MaximumImportCharacters = 50_000;
    private const int MaximumItemsPerField = 25;
    private const int MaximumItemCharacters = 200;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly ConcurrentDictionary<Guid, ProfileImport> _imports = new();

    public ProfileImport Create(AssistantProvider provider, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Imported assistant context cannot be empty.");
        }

        if (content.Length > MaximumImportCharacters)
        {
            throw new ArgumentException(
                $"Imported assistant context cannot exceed {MaximumImportCharacters:N0} characters.");
        }

        var proposedProfile = ExtractProfile(content);
        var import = new ProfileImport
        {
            BuilderId = builderProfileStore.Get().Id,
            Provider = provider,
            ProposedProfile = proposedProfile,
            Changes = BuildChanges(builderProfileStore.Get(), proposedProfile),
        };

        if (!_imports.TryAdd(import.Id, import))
        {
            throw new InvalidOperationException("Unable to create the profile import.");
        }

        // The untrusted pasted content is parsed synchronously and never retained.
        return import;
    }

    public ProfileImport Get(Guid id) => GetRequired(id);

    public ProfileImport Update(Guid id, ProposedBuilderProfile proposedProfile)
    {
        var import = GetRequired(id);
        EnsureEditable(import);

        import.ProposedProfile = Normalize(proposedProfile);
        import.Changes = BuildChanges(builderProfileStore.Get(), import.ProposedProfile);

        return import;
    }

    public BuilderProfile Approve(Guid id)
    {
        var import = GetRequired(id);
        EnsureEditable(import);

        var profile = builderProfileStore.ReplaceWith(import.ProposedProfile);
        import.Status = ProfileImportStatus.Approved;
        import.ApprovedAt = DateTimeOffset.UtcNow;
        import.Changes = [];

        return profile;
    }

    public ProfileImport Reject(Guid id)
    {
        var import = GetRequired(id);
        EnsureEditable(import);
        import.Status = ProfileImportStatus.Rejected;
        return import;
    }

    public void Delete(Guid id)
    {
        if (!_imports.TryRemove(id, out _))
        {
            throw new KeyNotFoundException("Profile import was not found.");
        }
    }

    private static ProposedBuilderProfile ExtractProfile(string content)
    {
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');

        if (start < 0 || end <= start)
        {
            throw new ArgumentException(
                "The imported context must contain the JSON object produced by the profile prompt.");
        }

        var json = content[start..(end + 1)];

        try
        {
            var profile = JsonSerializer.Deserialize<ProposedBuilderProfile>(
                json,
                SerializerOptions);

            return profile is null
                ? throw new ArgumentException("The imported profile JSON is empty.")
                : Normalize(profile);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException(
                "The imported context does not contain valid profile JSON.",
                exception);
        }
    }

    private static ProposedBuilderProfile Normalize(ProposedBuilderProfile profile)
    {
        var name = NormalizeRequired(profile.Name, "Name");

        return new ProposedBuilderProfile
        {
            Name = name,
            CurrentSkills = NormalizeList(profile.CurrentSkills),
            LearningGoals = NormalizeList(profile.LearningGoals),
            Interests = NormalizeList(profile.Interests),
            PreferredOpportunityTypes = NormalizeList(profile.PreferredOpportunityTypes),
            Location = NormalizeOptional(profile.Location),
            EffortPreferences = NormalizeList(profile.EffortPreferences),
        };
    }

    private static string NormalizeRequired(string? value, string field)
    {
        var normalized = NormalizeOptional(value);
        return normalized
            ?? throw new ArgumentException($"{field} is required.");
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length <= MaximumItemCharacters
            ? normalized
            : normalized[..MaximumItemCharacters];
    }

    private static List<string> NormalizeList(IEnumerable<string>? values) =>
        (values ?? [])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => NormalizeOptional(value)!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaximumItemsPerField)
            .ToList();

    private static IReadOnlyList<ProfileFieldChange> BuildChanges(
        BuilderProfile current,
        ProposedBuilderProfile proposed) =>
        [
            Change("Name", [current.Name], [proposed.Name]),
            Change("Current skills", current.CurrentSkills, proposed.CurrentSkills),
            Change("Learning goals", current.LearningGoals, proposed.LearningGoals),
            Change("Interests", current.Interests, proposed.Interests),
            Change(
                "Preferred opportunity types",
                current.PreferredOpportunityTypes,
                proposed.PreferredOpportunityTypes),
            Change(
                "Location",
                current.Location is null ? [] : [current.Location],
                proposed.Location is null ? [] : [proposed.Location]),
            Change(
                "Effort preferences",
                current.EffortPreferences,
                proposed.EffortPreferences),
        ];

    private static ProfileFieldChange Change(
        string field,
        IEnumerable<string> current,
        IEnumerable<string> proposed) =>
        new(field, current.ToArray(), proposed.ToArray());

    private ProfileImport GetRequired(Guid id) =>
        _imports.TryGetValue(id, out var import)
            ? import
            : throw new KeyNotFoundException("Profile import was not found.");

    private static void EnsureEditable(ProfileImport import)
    {
        if (import.Status != ProfileImportStatus.Extracted)
        {
            throw new InvalidOperationException(
                $"A {import.Status.ToString().ToLowerInvariant()} profile import cannot be changed.");
        }
    }
}
