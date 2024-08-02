using System.Threading.Tasks;
using System.Threading;

namespace IracingSdkDotNet.Extensions;

internal static class AutoResetEventExtensions
{
    public static Task<bool> WaitOneAsync(this AutoResetEvent autoResetEvent, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();

        var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
            autoResetEvent,
            (state, timedOut) => ((TaskCompletionSource<bool>)state!).TrySetResult(!timedOut),
            tcs,
            Timeout.InfiniteTimeSpan,
            true);

        if (cancellationToken != default)
        {
            cancellationToken.Register(() =>
            {
                tcs.TrySetResult(false);
                registeredWaitHandle.Unregister(null);
            });
        }

        return tcs.Task.ContinueWith(t =>
        {
            registeredWaitHandle.Unregister(null);
            return t.Result;
        });
    }
}
