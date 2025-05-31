using Microsoft.UI.Dispatching;
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
        public static async Task EnqueueAsync(this DispatcherQueue dispatcher, Action action)
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
        public static Task RunBackground(this DispatcherQueue dispatcher, Action action, Action? onComplete)
        {
            return Task.Run(async () => {
                action();
                if (onComplete != null) {
                    await dispatcher.EnqueueAsync(() =>
                    {
                        onComplete();
                    });
                }
                
            });
            
        }

        public static BackgroundTaskQueue BackgroundTaskQueue(this DispatcherQueue dispatcher) {
            return new BackgroundTaskQueue(dispatcher);
        }
    }
}
