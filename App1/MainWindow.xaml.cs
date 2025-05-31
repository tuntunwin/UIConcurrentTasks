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
        Task? _task;
        int _currentCount = 0;
        int _counter = 0;
        public MainWindow()
        {
            InitializeComponent();

        }

        public int CurrentCount;
        //{
        //    get => { _currentCount; }
        //    set => {
        //        _currentCount = value;
        //        Debug.WriteLine("Current Count:" + _currentCount);
        //    }
        //}

        private async void DoActionWhenNoBackgroundTask(Action action, Action onComplete = null)
        {
            if (_task != null)
            {
                Debug.WriteLine("Background task is running, waiting for it to complete...");
                await _task;
                Debug.WriteLine("Task is completed");
            }
            Debug.WriteLine("Task is null. Creating new background task ...");
            _task = DispatcherQueue.RunBackground(action, () =>
            {
                _task = null;
                onComplete?.Invoke();
            });
            Debug.WriteLine("Task is is created ...");
            //action();
            //onComplete();


            //if (_task == null)
            //{


            //}
            //else
            //{
            //    await _task;
            //    action();
            //    onComplete();
            //    //DoActionWhenNoBackgroundTask(action, onComplete);
            //    //await DispatcherQueue.EnqueueAsync(() => DoActionWhenNoBackgroundTask(action, onComplete));

            //}
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            var current = _counter++;
            DoActionWhenNoBackgroundTask(() =>
            {

                Debug.WriteLine($"Starting player ...");
                Task.Delay(2000).Wait();
                Debug.WriteLine($"Started player");
            }, () => { CurrentCount = current; });
            
        }

        //private async Task TestConfigureAwaite() {
        //    await Task.Delay(5000).ConfigureAwait(true);
        //    Debug.WriteLine("Start Clicked");
        //}
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Stop Clicked");
            var current = _counter++;
            DoActionWhenNoBackgroundTask(() =>
            {

                Debug.WriteLine($"Stopping player ...");
                Task.Delay(2000).Wait();
                Debug.WriteLine($"Stopped player");
            }, () => { CurrentCount = current; });

        }
    }
}
