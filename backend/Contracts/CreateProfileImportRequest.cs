using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed class CreateProfileImportRequest
{
    public required AssistantProvider Provider { get; init; }

    public required string Content { get; init; }
}
