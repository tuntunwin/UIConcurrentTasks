using System.ComponentModel;
using System.Windows.Input;

namespace App1
{
    /// <summary>
    /// Interface defining the common functionality for system control view models
    /// </summary>
    public interface ISystemControlViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the volume level between 0 and 100
        /// </summary>
        int VolumeLevel { get; set; }

        /// <summary>
        /// Gets whether the microphone is muted
        /// </summary>
        bool IsMicMuted { get; }

        /// <summary>
        /// Gets a command to change the volume level
        /// </summary>
        ICommand SetVolumeLevelCommand { get; }

        /// <summary>
        /// Gets a command to toggle the microphone mute state
        /// </summary>
        ICommand ToggleMicMuteCommand { get; }

        /// <summary>
        /// Gets whether the controls are enabled for user interaction
        /// </summary>
        bool AreControlsEnabled { get; }
    }
}