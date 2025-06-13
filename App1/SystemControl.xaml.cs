using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace App1
{
    public sealed partial class SystemControl : UserControl
    {
        private AutoSystemControlViewModel _autoViewModel;
        private ManualSystemControlViewModel _manualViewModel;
        private BackgroundTaskQueue _backgroundTaskQueue;
        private Random _random = new Random();

        // Note: These fields are actually defined in the generated code
        // private Slider VolumeSlider;
        // private bool _contentLoaded;

        //public static readonly DependencyProperty ViewModelProperty =
        //    DependencyProperty.Register(
        //        nameof(ViewModel),
        //        typeof(ISystemControlViewModel),
        //        typeof(SystemControl),
        //        new PropertyMetadata(null));

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                nameof(Mode),
                typeof(SystemControlMode),
                typeof(SystemControl),
                new PropertyMetadata(SystemControlMode.Manual, OnModeChanged));

        /// <summary>
        /// Gets or sets the current view model
        /// </summary>
        private ISystemControlViewModel ViewModel
        {
            //get => (ISystemControlViewModel)GetValue(ViewModelProperty);
            set
            {
                //SetValue(ViewModelProperty, value);
                this.DataContext = value; // Set DataContext to the ViewModel
            }
        }

        /// <summary>
        /// Gets or sets the current mode (Auto or Manual)
        /// </summary>
        public SystemControlMode Mode   
        {
            get => (SystemControlMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public SystemControl()
        {
            this.InitializeComponent();

            // Initialize view models
            _autoViewModel = new AutoSystemControlViewModel();
            _manualViewModel = new ManualSystemControlViewModel();

            // Set default view model
            ViewModel = _manualViewModel;

            this.Loaded += SystemControl_Loaded;
            this.Unloaded += SystemControl_Unloaded;
        }

        // Note: This method is generated in the auto-generated code file
        // private void InitializeComponent()
        // {
        //     if (_contentLoaded)
        //         return;
        //     _contentLoaded = true;
        //     global::Microsoft.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///SystemControl.xaml"), global::Microsoft.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
        // }

        private void SystemControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize background task queue
            _backgroundTaskQueue = DispatcherQueue.BackgroundTaskQueue();

            // Simulate system updates for auto mode
            StartAutoUpdateSimulation();
        }

        private void SystemControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up if needed
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SystemControl)d;
            control.UpdateViewModel();
        }

        private void UpdateViewModel()
        {
            // Update view model based on mode
            if (Mode == SystemControlMode.Auto)
            {
                // Copy current state to auto view model
                _autoViewModel.UpdateSystemState(
                    _manualViewModel.VolumeLevel,
                    _manualViewModel.IsMicMuted);

                ViewModel = _autoViewModel;
            }
            else
            {
                ViewModel = _manualViewModel;
            }
        }

        private async void StartAutoUpdateSimulation()
        {

            await Task.Delay(2000);
            Debug.WriteLine("Auto update simulation started");

            // Periodic updates
            while (true)
            {
                if (Mode == SystemControlMode.Auto)
                {
                    int newVolume = _random.Next(0, 101);
                    bool newMute = _random.Next(0, 2) == 1;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        // Update the auto view model with new values
                        _autoViewModel.UpdateSystemState(newVolume, newMute);
                    });
                    
                }

                await Task.Delay(3000);
            }
        }

        private void ManualMode_Click(object sender, RoutedEventArgs e)
        {
            Mode = SystemControlMode.Manual;
        }

        private void AutoMode_Click(object sender, RoutedEventArgs e)
        {
            Mode = SystemControlMode.Auto;
        }
    }

    /// <summary>
    /// Enum defining the control modes
    /// </summary>
    public enum SystemControlMode
    {
        /// <summary>
        /// Manual mode - interactive controls
        /// </summary>
        Manual,

        /// <summary>
        /// Auto mode - read-only display
        /// </summary>
        Auto
    }
}