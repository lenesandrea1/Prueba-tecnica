namespace EventosVivos.Tests.Unit;

internal sealed class FakeTimeProvider(DateTime utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(utcNow, TimeSpan.Zero);
}
