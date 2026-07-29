namespace ArgosHound.Api.Contracts;

public sealed class CreateSourceCommentRequest
{
    public required string ExternalId { get; init; }

    public string? ParentExternalId { get; init; }

    public required string Body { get; init; }

    public required string Url { get; init; }

    public string? AuthorHandle { get; init; }

    public required DateTimeOffset PublishedAt { get; init; }
}
