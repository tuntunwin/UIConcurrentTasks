using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public enum MediaPlayerStatus
    {
        Pending = 0,
        Buffering,
        Playing,
        Error,
        Stopped
    }

    public class MediaPlayerSimulator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Correctly declare and initialize the Logger field
        Random random = new Random();
        private async Task RandomDelay() => await Task.Delay(random.Next(2000, 3000)); // Random delay between 1 and 5 seconds
        public event EventHandler<MediaPlayerStatus>? StatusChanged;
        bool isPlaying = false;
        Task playbackTask = null;

        public void Play()
        {
            Logger.Debug($"{this.GetHashCode()} Playback starting...");
            RandomDelay().Wait(); // Simulate random delay before playback starts
            this.isPlaying = true;
            RaiseEvent(MediaPlayerStatus.Playing);
            playbackTask = StartPlaybackWithRandomEvents();
            Logger.Debug($"{this.GetHashCode()} Playback finished.");
        }

        public async Task StartPlaybackWithRandomEvents()
        {
            while (isPlaying)
            {
                var newStatus = (MediaPlayerStatus)random.Next(1, 4);
                await RaiseEventRandom(newStatus);
                if (newStatus == MediaPlayerStatus.Error)
                {
                    Logger.Error($"{this.GetHashCode()} Playback encountered an error.");
                    await RaiseEventRandom(MediaPlayerStatus.Error);
                    break; // Stop playback on error
                }
            }
        }

        private async Task RaiseEventRandom(MediaPlayerStatus status)
        {
            await RandomDelay();
            RaiseEvent(status);
        }

        private void RaiseEvent(MediaPlayerStatus status)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                Task.Run(() => handler(this, status));
            }
        }

        public void Stop()
        {
            if (isPlaying)
            {
                Logger.Debug($"{this.GetHashCode()} Stop starting...");
                isPlaying = false; // Stop playback 
                if (playbackTask != null && !playbackTask.IsCompleted)
                {
                    playbackTask.Wait(); // Wait for the playback task to complete
                }
                RandomDelay().Wait(); // Simulate stop delay
                RaiseEventRandom(MediaPlayerStatus.Stopped);
                Logger.Debug($"{this.GetHashCode()} Stop finished.");
            }
        }
    }
}
