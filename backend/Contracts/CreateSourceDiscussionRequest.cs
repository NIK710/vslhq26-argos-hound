namespace ArgosHound.Api.Contracts;

public sealed class CreateSourceDiscussionRequest
{
    public required string Platform { get; init; }

    public required string ExternalId { get; init; }

    public required string Community { get; init; }

    public required string Title { get; init; }

    public required string Body { get; init; }

    public required string Url { get; init; }

    public string? AuthorHandle { get; init; }

    public required DateTimeOffset PublishedAt { get; init; }

    public IReadOnlyList<CreateSourceCommentRequest> Comments { get; init; } = [];
}
