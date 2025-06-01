using Microsoft.UI.Dispatching;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace App1
{
    public class BackgroundTaskQueue
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Correctly declare and initialize the Logger field
        private Task? task;
        
        private DispatcherQueue dispatcherQueue;
        public BackgroundTaskQueue(DispatcherQueue dispatcherQueue) {
            this.dispatcherQueue = dispatcherQueue;
        }

        public void EnsureAccess() {
            if (!this.dispatcherQueue.HasThreadAccess) {
                throw new InvalidOperationException("Invalid thread to acccess");
            }
        }
        public void EnqueTaskNoWait(Action action, Action? onComplete = null) {
            var _ = EnqueTask(action, onComplete);
        }

        public void EnqueDelayTask(int delay, Action onComplete)
        {
            var createTask = () => this.dispatcherQueue.RunDelay(delay, () =>
            {
                this.task = null;
                onComplete?.Invoke();
            });

            var _ = EnqueTask(createTask);
        }

        public async void EnqueDispatcher(Action action) { 
            await this.dispatcherQueue.EnqueueAsync(action);
        }
        public async Task EnqueTask(Action action, Action? onComplete = null)
        {
            var createTask = () => this.dispatcherQueue.RunBackground(action, () =>
            {
                this.task = null;
                onComplete?.Invoke();
            });
            await EnqueTask(createTask);


        }

        public async Task EnqueTask(Func<Task> createTask)
        {
            Assert.True(this.dispatcherQueue.HasThreadAccess, "This method must be called on the DispatcherQueue thread.");
            while (this.task != null)
            {
                Logger.Debug("Background task is running, waiting for it to complete...");
                await this.task;
                Logger.Debug("Awaite is completed");
            }
            Logger.Debug("Task is null. Creating new background task ...");
            this.task = createTask();
            Logger.Debug("Task is is created ...");

        }
    }
}
