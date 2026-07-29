namespace ArgosHound.Api.Models;

public sealed class SourceDiscussion
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Platform { get; init; }

    public required string ExternalId { get; init; }

    public required string Community { get; init; }

    public required string Title { get; init; }

    public required string Body { get; init; }

    public required string Url { get; init; }

    public string? AuthorHandle { get; init; }

    public required DateTimeOffset PublishedAt { get; init; }

    public DateTimeOffset RetrievedAt { get; init; } = DateTimeOffset.UtcNow;

    public IReadOnlyList<SourceComment> Comments { get; init; } = [];
}
