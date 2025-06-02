using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class PlayerView : UserControl
    {
        public static readonly DependencyProperty DeviceIdProperty =
       DependencyProperty.Register(
       nameof(DeviceId),
       typeof(string),
       typeof(PlayerView),
       new PropertyMetadata(default(string), OnDeviceIdChanged));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
            nameof(Status),
            typeof(CurrentPlayerStatus),
            typeof(PlayerView),
            new PropertyMetadata(CurrentPlayerStatus.Idle));
        public string DeviceId
        {
            get => (string)GetValue(DeviceIdProperty);
            set => SetValue(DeviceIdProperty, value);
        }

        public CurrentPlayerStatus Status
        {
            get => (CurrentPlayerStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        private static void OnDeviceIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlayerView)d;
            control.OnDeviceIdChanged((string?)e.OldValue, (string?)e.NewValue);
        }

        private async void OnDeviceIdChanged(string? oldId, string? newId)
        {
        }

        private IPlayerController controller;
        public PlayerView()
        {
            InitializeComponent();
            this.Loaded += PlayerView_Loaded;
            this.Unloaded += PlayerView_Unloaded;
        }

        private void PlayerView_Loaded(object sender, RoutedEventArgs e)
        {
            this.controller = new SimplePlayerController(DeviceId, new MediaPlayerSimulator(), DispatcherQueue.BackgroundTaskQueue());
            this.controller.CurrentStatusChanged += Controller_CurrentStatusChanged;
            this.controller.NextTarget(TargetPlayerStatus.Playing);
        }

        private void Controller_CurrentStatusChanged(object? sender, CurrentPlayerStatus e)
        {
            this.Status = e;
        }

        private void PlayerView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.controller.NextTarget(TargetPlayerStatus.Stopped);
        }

       
    }
}
