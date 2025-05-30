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
        private const string V = "Running";
        Task? _task;
        string _status = "Idle";
        public MainWindow()
        {
            InitializeComponent();
          
        }

        

        private async void DoActionWhenNoBackgroundTask(string name, Action action)
        {
            if(_task == null)
            {
                Debug.WriteLine($"Calling action {name}");
                await RunBackgraoundTask(name, action);
                Debug.WriteLine($"Finish calling action {name}");
            }
            else
            {
                await Task.Delay(1000).ConfigureAwait(false);
                await _task.ContinueWith(t =>
                {
                    Debug.WriteLine($"Retrying action {name} after task completion");
                    DispatcherQueue.TryEnqueue(() => DoActionWhenNoBackgroundTask(name, action));
                }).ConfigureAwait(false);

            }
        }

        async Task RunBackgraoundTask(string name, Action action)
        {
            Debug.WriteLine($"Starting background task {name} ...");
            _task = Task.Run(action);
            await _task.ConfigureAwait(false);
            Debug.WriteLine($"Complete task {name} ...");
            DispatcherQueue.TryEnqueue(() =>
            {
                // Update UI after background task completion
                Debug.WriteLine($"Cleaning task {name} ...");
                _task = null;
            });
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            DoActionWhenNoBackgroundTask("START", () =>
            {
                Debug.WriteLine($"Starting player ...");
                Task.Delay(2000).Wait();
                Debug.WriteLine($"Started player");
            });
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            DoActionWhenNoBackgroundTask("STOP", () =>
            {
                Debug.WriteLine($"Stopping player ...");
                Task.Delay(3000).Wait();
                Debug.WriteLine($"Stopped player");
            });

        }
    }
}
