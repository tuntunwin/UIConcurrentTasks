using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace App1
{

    public class SimplePlayerController : IPlayerController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private MediaPlayerSimulator player;
        private BackgroundTaskQueue backgroundTaskQueue;

        CurrentPlayerStatus currentStatus;
        TargetPlayerStatus targetStatus;

        public CurrentPlayerStatus CurrentStatus { get => currentStatus; }
        public TargetPlayerStatus TargetStatus { get => targetStatus; }
        private string playerId;

        public event EventHandler<CurrentPlayerStatus> CurrentStatusChanged;
        static readonly Dictionary<MediaPlayerStatus, CurrentPlayerStatus> MediaPlayerStatusMap = new() {
            { MediaPlayerStatus.Pending,   CurrentPlayerStatus.Idle },
            { MediaPlayerStatus.Buffering, CurrentPlayerStatus.Bufering },
            { MediaPlayerStatus.Playing,   CurrentPlayerStatus.Playing },
            { MediaPlayerStatus.Error,     CurrentPlayerStatus.Error },
            { MediaPlayerStatus.Stopped,   CurrentPlayerStatus.Stopped },
        };

        public SimplePlayerController(string playerId, MediaPlayerSimulator player, BackgroundTaskQueue backgroundTaskQueue) { 
            this.playerId = playerId;
            this.player = player;
            this.backgroundTaskQueue = backgroundTaskQueue;
            this.player.StatusChanged += Player_StatusChanged;
        }

        private void Player_StatusChanged(object? sender, MediaPlayerStatus e)
        {
            this.backgroundTaskQueue.EnqueDispatcher(() => NextCurrent(MediaPlayerStatusMap[e]));
        }

        public void NextTarget(TargetPlayerStatus targetStatus)
        {
            if (this.targetStatus == targetStatus)
            {
                Logger.Debug($"{this.playerId}: Target status is already {targetStatus}. No action taken.");
                return;
            }
            if (targetStatus == TargetPlayerStatus.Playing)
            {
                Play();
            }
            else if (targetStatus == TargetPlayerStatus.Stopped)
            {
                Stop();
            }
            else
            {
                Logger.Warn($"{this.playerId}: Unsupported target status: {targetStatus}. No action taken.");
            }
        }

        public void NextCurrent(CurrentPlayerStatus currentStatus)
        {
            this.backgroundTaskQueue.EnsureAccess();
            if (this.currentStatus != currentStatus)
            {
                this.currentStatus = currentStatus;
                Logger.Debug($"{this.playerId}: Current Status = {this.currentStatus}");
                CurrentStatusChanged?.Invoke(this, this.currentStatus);
            }
        }

        public void Play()
        {
            if(this.targetStatus != TargetPlayerStatus.Idle)
            {
                Logger.Warn($"{this.playerId}: Player must be in Idle status to play. Current is {this.targetStatus}");
                return;
            }
            this.targetStatus = TargetPlayerStatus.Playing;
            StartPlayer();

        }

        public void Stop()
        {
            switch (this.targetStatus)
            {
                case TargetPlayerStatus.Idle:
                    Logger.Warn($"{this.playerId}: Player is in Idle status and no need to stop");
                    return;
                case TargetPlayerStatus.Stopped:
                    Logger.Warn($"{this.playerId}: Player is already stopped");
                    return;
                
            }
            //Target status is Playing. Must stop it
            this.targetStatus = TargetPlayerStatus.Stopped;
            switch (this.currentStatus)
            {
                case CurrentPlayerStatus.Starting:
                    Logger.Debug($"{this.playerId}: Player is starting. will continue after started");
                    break;
                case CurrentPlayerStatus.Playing:
                case CurrentPlayerStatus.Bufering:
                case CurrentPlayerStatus.Error:
                    StopPlayer();
                    return;
                default:
                    Logger.Warn($"{this.playerId}: Player is not in a state that can be stopped. Current status: {this.currentStatus}");
                    return;
            }

        }

        private void StartPlayer()
        {
            NextCurrent(CurrentPlayerStatus.Starting);
            Task.Run(() => this.player.Play()).ContinueWith(t =>
            {
                this.backgroundTaskQueue.EnqueDispatcher(() => {
                    if (t.IsFaulted)
                    {
                        Logger.Error($"{this.playerId}: Playback failed with error: {t.Exception?.GetBaseException().Message}");
                        NextCurrent(CurrentPlayerStatus.Error);
                    }
                    else
                    {
                        this.currentStatus = CurrentPlayerStatus.Started;
                    }
                    if (this.targetStatus == TargetPlayerStatus.Stopped)
                    {
                        Logger.Debug($"{this.playerId}: Target status has been changed to Stopped. going to stop");
                        StopPlayer();
                    }
                    
                });
            });
        }

        private void StopPlayer()
        {
            NextCurrent(CurrentPlayerStatus.Stopping);
            Task.Run(() => this.player.Stop()).ContinueWith(t =>
            {
                this.backgroundTaskQueue.EnqueDispatcher(() => {
                    if (t.IsFaulted)
                    {
                        Logger.Error($"{this.playerId}: Stop failed with error: {t.Exception?.GetBaseException().Message}");
                        NextCurrent(CurrentPlayerStatus.Error);
                    }
                    else
                    {
                        NextCurrent(CurrentPlayerStatus.Stopped);
                    }
                });
            });
        }

        
    }
}
