using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TlsClient.Core.Helpers
{
    public class AsyncHelpers
    {
        public static void RunSync(Func<Task> taskFactory, bool forceThreadPool = true, CancellationToken ct = default)
        {
            if (taskFactory is null) throw new ArgumentNullException(nameof(taskFactory));
            if (ct.IsCancellationRequested) throw new OperationCanceledException(ct);

            if (forceThreadPool)
            {
                Task.Run(async () =>
                {
                    ct.ThrowIfCancellationRequested();
                    await taskFactory().ConfigureAwait(false);
                }, ct).GetAwaiter().GetResult();
            }
            else
            {
                taskFactory().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public static T RunSync<T>(Func<Task<T>> taskFactory, bool forceThreadPool = true, CancellationToken ct = default)
        {
            if (taskFactory is null) throw new ArgumentNullException(nameof(taskFactory));
            if (ct.IsCancellationRequested) throw new OperationCanceledException(ct);

            if (forceThreadPool)
            {
                return Task.Run(async () =>
                {
                    ct.ThrowIfCancellationRequested();
                    return await taskFactory().ConfigureAwait(false);
                }, ct).GetAwaiter().GetResult();
            }
            else
            {
                return taskFactory().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public static Task RunAsync(Action action, CancellationToken ct = default)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            return Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();
                action();
            }, ct);
        }

        public static Task<T> RunAsync<T>(Func<T> func, CancellationToken ct = default)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            return Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();
                return func();
            }, ct);
        }
    }
}
