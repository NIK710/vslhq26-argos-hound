using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed class UpdateProfileImportRequest
{
    public required ProposedBuilderProfile ProposedProfile { get; init; }
}
