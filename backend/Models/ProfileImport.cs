namespace ArgosHound.Api.Models;

public sealed class ProfileImport
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid BuilderId { get; init; }

    public required AssistantProvider Provider { get; init; }

    public ProfileImportStatus Status { get; set; } = ProfileImportStatus.Extracted;

    public required ProposedBuilderProfile ProposedProfile { get; set; }

    public IReadOnlyList<ProfileFieldChange> Changes { get; set; } = [];

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateTimeOffset RawContentDeletedAt { get; init; } = DateTimeOffset.UtcNow;
}
