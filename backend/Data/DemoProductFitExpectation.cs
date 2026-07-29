namespace ArgosHound.Api.Data;

public enum DemoFitStrength
{
    Direct,
    Adjacent,
    Weak,
}

public sealed record DemoProductFitExpectation(
    Guid ProductId,
    DemoFitStrength ExpectedFit,
    string Rationale);
