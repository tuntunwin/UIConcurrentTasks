using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App1
{
    /// <summary>
    /// Base class for system control view models implementing INotifyPropertyChanged
    /// </summary>
    public abstract class SystemControlViewModelBase : ISystemControlViewModel
    {
        private int _volumeLevel;
        private bool _isMicMuted;

        /// <summary>
        /// Event that is fired when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the volume level between 0 and 100
        /// </summary>
        public int VolumeLevel
        {
            get => _volumeLevel;
            set
            {
                if (_volumeLevel != value)
                {
                    _volumeLevel = Math.Clamp(value, 0, 100);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the microphone is muted
        /// </summary>
        public bool IsMicMuted
        {
            get => _isMicMuted;
            protected set
            {
                if (_isMicMuted != value)
                {
                    _isMicMuted = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a command to change the volume level
        /// </summary>
        public abstract ICommand SetVolumeLevelCommand { get; }

        /// <summary>
        /// Gets a command to toggle the microphone mute state
        /// </summary>
        public abstract ICommand ToggleMicMuteCommand { get; }

        /// <summary>
        /// Gets whether the controls are enabled for user interaction
        /// </summary>
        public abstract bool AreControlsEnabled { get; }

        /// <summary>
        /// Called when a property changes to notify the UI
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}