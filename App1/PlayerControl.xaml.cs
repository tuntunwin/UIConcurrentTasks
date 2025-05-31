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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class PlayerControl : UserControl
    {
        public static readonly DependencyProperty DeviceIdProperty =
            DependencyProperty.Register(
            nameof(DeviceId),
            typeof(string),
            typeof(PlayerControl),
            new PropertyMetadata(default(string), OnDeviceIdChanged));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
            nameof(Status),
            typeof(string),
            typeof(PlayerControl),
            new PropertyMetadata("Pending"));

        public string DeviceId
        {
            get => (string)GetValue(DeviceIdProperty);
            set => SetValue(DeviceIdProperty, value);
        }

        public String Status
        {
            get => (String)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        private MediaPlayerSimulator player;
        private BackgroundTaskQueue backgroundTaskQueue;
        public PlayerControl()
        {
            InitializeComponent();
            this.Loaded += PlayerControl_Loaded;    
            this.Unloaded += PlayerControl_Unloaded;
            // Player will be created on DeviceId set
        }

        private async void PlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            await StopPlayer();
        }

        private void PlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private static void OnDeviceIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlayerControl)d;
            control.OnDeviceIdChanged((string?)e.OldValue, (string?)e.NewValue);
        }

        private async void OnDeviceIdChanged(string? oldId, string? newId)
        {
            if (this.player != null)
            {
                var _ = StopPlayer();
            }
            await StartPlayer();


        }

        private async Task StopPlayer() {
            this.player.StatusChanged -= Player_StatusChanged;
            var player = this.player;
            var backgroundTaskQueue = this.backgroundTaskQueue;
            this.player = null;
            this.backgroundTaskQueue = null;

            await backgroundTaskQueue.Enque(player.Stop);
        }

        private async Task StartPlayer()
        {
           
            // Create new player
            this.backgroundTaskQueue = DispatcherQueue.BackgroundTaskQueue();
            player = new MediaPlayerSimulator();
            Status = MediaPlayerStatus.Pending.ToString();
            player.StatusChanged += Player_StatusChanged;
            // Optionally, start playing automatically
            await backgroundTaskQueue.Enque(player.Play);
        }

        private async Task RestartPlayer()
        {
            if(this.player != null)
            {
                this.player.StatusChanged -= Player_StatusChanged;
                
            }
            // Create new player
            this.backgroundTaskQueue = DispatcherQueue.BackgroundTaskQueue();
            player = new MediaPlayerSimulator();
            Status = "Retrying";
            player.StatusChanged += Player_StatusChanged;
            // Optionally, start playing automatically
            await backgroundTaskQueue.Enque(player.Play);
        }

        private async void Player_StatusChanged(object? sender, MediaPlayerStatus e)
        {
            var _  = DispatcherQueue.EnqueueAsync(async () =>
            {
                if (this.player == sender) {
                    Status = e.ToString();
                    if (e == MediaPlayerStatus.Error)
                    {
                        // Handle error, e.g., show a message to the user
                        Debug.WriteLine($"Player {DeviceId} encountered an error. Retrying Play");
                        await Task.Delay(2000); // Wait before retrying
                        await RestartPlayer();
                    }

                }
            });
        }
    }
}
