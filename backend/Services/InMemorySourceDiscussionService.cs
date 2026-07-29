using System.Collections.Concurrent;
using ArgosHound.Api.Contracts;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class InMemorySourceDiscussionService : ISourceDiscussionService
{
    private const int MaximumComments = 100;
    private const int MaximumExternalIdLength = 200;
    private const int MaximumCommunityLength = 200;
    private const int MaximumTitleLength = 300;
    private const int MaximumBodyLength = 20_000;
    private const int MaximumHandleLength = 100;

    private readonly ConcurrentDictionary<Guid, SourceDiscussion> _discussions =
        new(DemoSourceData.Discussions.ToDictionary(item => item.Id, Clone));

    public IReadOnlyList<SourceDiscussion> GetAll() =>
        _discussions.Values
            .OrderByDescending(item => item.PublishedAt)
            .Select(Clone)
            .ToArray();

    public SourceDiscussion Get(Guid id) =>
        _discussions.TryGetValue(id, out var discussion)
            ? Clone(discussion)
            : throw new KeyNotFoundException("Source discussion was not found.");

    public SourceDiscussion Create(CreateSourceDiscussionRequest request)
    {
        Validate(request);

        var platform = NormalizeRequired(
            request.Platform,
            nameof(request.Platform),
            MaximumCommunityLength);
        var externalId = NormalizeRequired(
            request.ExternalId,
            nameof(request.ExternalId),
            MaximumExternalIdLength);

        if (_discussions.Values.Any(
                item =>
                    item.Platform.Equals(platform, StringComparison.OrdinalIgnoreCase)
                    && item.ExternalId.Equals(externalId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(
                "A source discussion with this platform and external ID already exists.");
        }

        var discussionId = Guid.NewGuid();
        var discussion = new SourceDiscussion
        {
            Id = discussionId,
            Platform = platform,
            ExternalId = externalId,
            Community = NormalizeRequired(
                request.Community,
                nameof(request.Community),
                MaximumCommunityLength),
            Title = NormalizeRequired(
                request.Title,
                nameof(request.Title),
                MaximumTitleLength),
            Body = NormalizeRequired(
                request.Body,
                nameof(request.Body),
                MaximumBodyLength),
            Url = NormalizeUrl(request.Url, nameof(request.Url)),
            AuthorHandle = NormalizeOptional(request.AuthorHandle, MaximumHandleLength),
            PublishedAt = request.PublishedAt,
            RetrievedAt = DateTimeOffset.UtcNow,
            Comments = request.Comments
                .Select(comment => new SourceComment
                {
                    DiscussionId = discussionId,
                    ExternalId = NormalizeRequired(
                        comment.ExternalId,
                        nameof(comment.ExternalId),
                        MaximumExternalIdLength),
                    ParentExternalId = NormalizeOptional(
                        comment.ParentExternalId,
                        MaximumExternalIdLength),
                    Body = NormalizeRequired(
                        comment.Body,
                        nameof(comment.Body),
                        MaximumBodyLength),
                    Url = NormalizeUrl(comment.Url, nameof(comment.Url)),
                    AuthorHandle = NormalizeOptional(
                        comment.AuthorHandle,
                        MaximumHandleLength),
                    PublishedAt = comment.PublishedAt,
                })
                .ToArray(),
        };

        if (!_discussions.TryAdd(discussion.Id, discussion))
        {
            throw new InvalidOperationException("Unable to store the source discussion.");
        }

        return Clone(discussion);
    }

    private static void Validate(CreateSourceDiscussionRequest request)
    {
        if (request.PublishedAt == default)
        {
            throw new ArgumentException("Discussion publishedAt is required.");
        }

        if (request.Comments.Count > MaximumComments)
        {
            throw new ArgumentException(
                $"A discussion cannot include more than {MaximumComments} comments.");
        }

        var commentIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var comment in request.Comments)
        {
            if (comment.PublishedAt == default)
            {
                throw new ArgumentException("Every comment requires publishedAt.");
            }

            if (!commentIds.Add(comment.ExternalId?.Trim() ?? string.Empty))
            {
                throw new ArgumentException(
                    "Comment external IDs must be non-empty and unique within a discussion.");
            }
        }
    }

    private static string NormalizeRequired(
        string? value,
        string field,
        int maximumLength)
    {
        var normalized = NormalizeOptional(value, maximumLength);
        return normalized ?? throw new ArgumentException($"{field} is required.");
    }

    private static string? NormalizeOptional(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException(
                $"A source field exceeds the maximum length of {maximumLength:N0} characters.");
        }

        return normalized;
    }

    private static string NormalizeUrl(string? value, string field)
    {
        var normalized = NormalizeRequired(value, field, 2_000);

        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException($"{field} must be an absolute HTTP or HTTPS URL.");
        }

        return uri.AbsoluteUri;
    }

    private static SourceDiscussion Clone(SourceDiscussion source) =>
        new()
        {
            Id = source.Id,
            Platform = source.Platform,
            ExternalId = source.ExternalId,
            Community = source.Community,
            Title = source.Title,
            Body = source.Body,
            Url = source.Url,
            AuthorHandle = source.AuthorHandle,
            PublishedAt = source.PublishedAt,
            RetrievedAt = source.RetrievedAt,
            Comments = source.Comments.Select(Clone).ToArray(),
        };

    private static SourceComment Clone(SourceComment source) =>
        new()
        {
            Id = source.Id,
            DiscussionId = source.DiscussionId,
            ExternalId = source.ExternalId,
            ParentExternalId = source.ParentExternalId,
            Body = source.Body,
            Url = source.Url,
            AuthorHandle = source.AuthorHandle,
            PublishedAt = source.PublishedAt,
        };
}
