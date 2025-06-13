using System.Windows.Input;

namespace App1
{
    /// <summary>
    /// View model for auto system control mode where controls are read-only
    /// </summary>
    public class AutoSystemControlViewModel : SystemControlViewModelBase
    {
        private readonly ICommand _emptyCommand;

        /// <summary>
        /// Initializes a new instance of the AutoSystemControlViewModel class
        /// </summary>
        /// <param name="initialVolumeLevel">Initial volume level</param>
        /// <param name="initialMicMuted">Initial microphone mute state</param>
        public AutoSystemControlViewModel(int initialVolumeLevel = 50, bool initialMicMuted = false)
        {
            VolumeLevel = initialVolumeLevel;
            IsMicMuted = initialMicMuted;
            _emptyCommand = new RelayCommand((_) => { }, (_) => false);
        }

        /// <summary>
        /// Gets a command to change the volume level (disabled in auto mode)
        /// </summary>
        public override ICommand SetVolumeLevelCommand => _emptyCommand;

        /// <summary>
        /// Gets a command to toggle the microphone mute state (disabled in auto mode)
        /// </summary>
        public override ICommand ToggleMicMuteCommand => _emptyCommand;

        /// <summary>
        /// Gets whether the controls are enabled for user interaction (always false in auto mode)
        /// </summary>
        public override bool AreControlsEnabled => false;

        /// <summary>
        /// Updates the system state from external system (used by the system to update UI)
        /// </summary>
        /// <param name="volumeLevel">New volume level</param>
        /// <param name="isMicMuted">New mic mute state</param>
        public void UpdateSystemState(int volumeLevel, bool isMicMuted)
        {
            VolumeLevel = volumeLevel;
            IsMicMuted = isMicMuted;
        }
    }
}