using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace App1
{
    public static class DispatcherQueueExtensions
    {
        public static async Task EnqueueAsync(this Microsoft.UI.Dispatching.DispatcherQueue dispatcher, Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            dispatcher.TryEnqueue(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            await tcs.Task;
        }
        public static Task RunBackground(this Microsoft.UI.Dispatching.DispatcherQueue dispatcher, Action action, Action onComplete)
        {
            //var context = SynchronizationContext.Current;
            return Task.Run(async () => {
                action();
                //context?.Send((_) =>
                //{
                //    onComplete?.Invoke();
                //}, null);
                await dispatcher.EnqueueAsync(() =>
                {
                    onComplete();
                });
            });
            
        }
    }
}
