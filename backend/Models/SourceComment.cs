namespace ArgosHound.Api.Models;

public sealed class SourceComment
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid DiscussionId { get; init; }

    public required string ExternalId { get; init; }

    public string? ParentExternalId { get; init; }

    public required string Body { get; init; }

    public required string Url { get; init; }

    public string? AuthorHandle { get; init; }

    public required DateTimeOffset PublishedAt { get; init; }
}
