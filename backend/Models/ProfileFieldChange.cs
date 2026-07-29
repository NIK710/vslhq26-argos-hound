namespace ArgosHound.Api.Models;

public sealed record ProfileFieldChange(
    string Field,
    IReadOnlyList<string> CurrentValues,
    IReadOnlyList<string> ProposedValues);
