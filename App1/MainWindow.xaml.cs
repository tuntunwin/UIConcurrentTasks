using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private int counter = 0;
        private int _currentCount = 0;
        
        public int CurrentCount
        {
            get => _currentCount;
            set
            {
                var oldValue = _currentCount;
                _currentCount = value;
                Debug.WriteLine($"Current Count: {oldValue} -> {_currentCount}");
            }
        }
        BackgroundTaskQueue backgroundTaskQueue;
        public MainWindow()
        {
            InitializeComponent();
            this.Activated += MainWindow_Activated;

        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            this.backgroundTaskQueue = DispatcherQueue.BackgroundTaskQueue();
        }


        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            var current = ++counter;
            await this.backgroundTaskQueue.Enque(() =>
            {
                Debug.WriteLine($"Starting player ...");
                Task.Delay(2000).Wait();
                Debug.WriteLine($"Started player");
            }, () => { CurrentCount = current; });
            
        }

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Stop Clicked");
            var current = ++counter;
            await this.backgroundTaskQueue.Enque(() =>
            {
                Debug.WriteLine($"Stopping player ...");
                Task.Delay(2000).Wait();
                Debug.WriteLine($"Stopped player");
            }, () => { CurrentCount = current; });

        }

       
    }
}
