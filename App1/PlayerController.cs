using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public enum TargetPlayerStatus { 
        Idle,
        Playing,
        Stopped
    }

    public enum CurrentPlayerStatus { 
        Idle,
        Starting,
        Started,
        Bufering,
        Playing,
        Restarting,
        Stopping,
        Stopped,
        Error
    }

    public class PlayerController
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

        public PlayerController(string playerId, MediaPlayerSimulator player, BackgroundTaskQueue backgroundTaskQueue) { 
            this.playerId = playerId;
            this.player = player;
            this.backgroundTaskQueue = backgroundTaskQueue;
            this.player.StatusChanged += Player_StatusChanged;
        }

        private void Player_StatusChanged(object? sender, MediaPlayerStatus e)
        {
            this.backgroundTaskQueue.EnqueDispatcher(() => this.NextCurrent(MediaPlayerStatusMap[e]));
        }

        public void NextCurrent(CurrentPlayerStatus currentStatus) {
            this.backgroundTaskQueue.EnsureAccess();
            if (this.currentStatus != currentStatus)
            {
                this.currentStatus = currentStatus;
                Logger.Debug($"{this.playerId}: Current Status = {this.currentStatus}");
                CurrentStatusChanged?.Invoke(this, this.currentStatus);
                Next();
                
            }
        }

        public void NextTarget(TargetPlayerStatus targetStatus)
        {
            this.backgroundTaskQueue.EnsureAccess();
            if (this.targetStatus != targetStatus) { 
                this.targetStatus = targetStatus;
                Logger.Debug($"{this.playerId}: Target Status = {this.targetStatus}");
                Next();
            }
        }

        private void Next() {
            if (targetStatus == TargetPlayerStatus.Playing &&
                (currentStatus != CurrentPlayerStatus.Playing || currentStatus != CurrentPlayerStatus.Bufering))
            {
                ContinuePlay();
            }
            else if (targetStatus == TargetPlayerStatus.Stopped &&
                (currentStatus != CurrentPlayerStatus.Stopped))
            {
                ContinueStop();
            }
        }

        private void ContinuePlay()
        {
            switch (currentStatus) {
                case CurrentPlayerStatus.Idle: 
                    StartPlayer();
                    break;

                case CurrentPlayerStatus.Starting:
                case CurrentPlayerStatus.Restarting:
                case CurrentPlayerStatus.Started:
                    //Wait until next schedule
                    break;

                case CurrentPlayerStatus.Stopping:
                case CurrentPlayerStatus.Stopped:
                    //Won't restart
                    break;
                
                case CurrentPlayerStatus.Error:
                    HandleError();
                    break;

                default:
                    //All Done. No more work
                    break;
            }

        }

        private void ContinueStop()
        {
            switch (currentStatus)
            {
                case CurrentPlayerStatus.Idle:
                    //No need to do anything
                    break;

                case CurrentPlayerStatus.Starting:
                case CurrentPlayerStatus.Restarting:
                case CurrentPlayerStatus.Stopping:
                    //Wait until next schedule
                    break;

                case CurrentPlayerStatus.Started:
                case CurrentPlayerStatus.Playing:
                case CurrentPlayerStatus.Bufering:
                    StopPlayer();
                    break;

                case CurrentPlayerStatus.Error:
                    HandleError();
                    break;

                default:
                    //All Done. No more work
                    break;
            }
        }

        private void StartPlayer()
        {
            this.backgroundTaskQueue.EnqueTaskNoWait(this.player.Play, () => this.NextCurrent(CurrentPlayerStatus.Started));
        }

        private void StopPlayer()
        {
            this.backgroundTaskQueue.EnqueTaskNoWait(this.player.Stop, () => this.NextCurrent(CurrentPlayerStatus.Stopped));
        }
        private void HandleError()
        {
            if (TargetStatus == TargetPlayerStatus.Playing) {
                StartPlayer();
            }
            else if(TargetStatus == TargetPlayerStatus.Stopped)
            {
                StopPlayer();
            }
        }

        



    }
}
