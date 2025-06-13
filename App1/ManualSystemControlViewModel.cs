using System;
using System.Diagnostics;
using System.Windows.Input;

namespace App1
{
    /// <summary>
    /// View model for manual system control mode where controls are interactive
    /// </summary>
    public class ManualSystemControlViewModel : SystemControlViewModelBase
    {
        private ICommand _setVolumeLevelCommand;
        private ICommand _toggleMicMuteCommand;

        /// <summary>
        /// Initializes a new instance of the ManualSystemControlViewModel class
        /// </summary>
        /// <param name="initialVolumeLevel">Initial volume level</param>
        /// <param name="initialMicMuted">Initial microphone mute state</param>
        public ManualSystemControlViewModel(int initialVolumeLevel = 50, bool initialMicMuted = false)
        {
            VolumeLevel = initialVolumeLevel;
            IsMicMuted = initialMicMuted;
            
            _setVolumeLevelCommand = new RelayCommand(
                (parameter) => 
                {
                    if (parameter is int newVolumeLevel)
                    {
                        VolumeLevel = newVolumeLevel;
                        Debug.WriteLine($"Volume set to {VolumeLevel}");
                    }
                },
                (parameter) => true
            );
            
            _toggleMicMuteCommand = new RelayCommand(
                (parameter) =>
                {
                    IsMicMuted = !IsMicMuted;
                    Debug.WriteLine($"Microphone mute toggled to {IsMicMuted}");
                },
                (parameter) => true
            );
        }

        /// <summary>
        /// Gets a command to change the volume level
        /// </summary>
        public override ICommand SetVolumeLevelCommand => _setVolumeLevelCommand;

        /// <summary>
        /// Gets a command to toggle the microphone mute state
        /// </summary>
        public override ICommand ToggleMicMuteCommand => _toggleMicMuteCommand;

        /// <summary>
        /// Gets whether the controls are enabled for user interaction (always true in manual mode)
        /// </summary>
        public override bool AreControlsEnabled => true;
    }
}