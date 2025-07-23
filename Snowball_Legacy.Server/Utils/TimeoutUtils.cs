namespace Snowball_Legacy.Server.Utils;

public static class TimeoutUtils
{
    private const int DefaultTimeoutSeconds = 15;

    public static CancellationTokenSource CreateTimeoutCts(CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(DefaultTimeoutSeconds));
        return cts;
    }
}
