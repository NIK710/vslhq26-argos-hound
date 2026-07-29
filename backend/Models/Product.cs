namespace ArgosHound.Api.Models;

public sealed class Product
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid BuilderId { get; init; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public List<string> Capabilities { get; set; } = [];

    public List<string> TargetUsers { get; set; } = [];

    public required string ProductUrl { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
