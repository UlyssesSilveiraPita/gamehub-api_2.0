namespace GameHub.Web.Tests.Helpers;

internal sealed class StubTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;

    public StubTimeProvider(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return _utcNow;
    }

    public void Advance(TimeSpan duration)
    {
        _utcNow = _utcNow.Add(duration);
    }
}