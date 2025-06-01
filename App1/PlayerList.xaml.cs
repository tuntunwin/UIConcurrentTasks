using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class PlayerList : UserControl, INotifyPropertyChanged
    {
        private int _currentPage = 0;
        public List<string> Devices { get; set; } = new List<string>();
        public event PropertyChangedEventHandler? PropertyChanged;
        public PlayerList()
        {
            InitializeComponent();
            this.DataContext = this;
            PageChange();
        }

        private void Next_Page_Click(object sender, RoutedEventArgs e)
        {
            this.Devices.Clear();
            PageChange();
        }

        private void PageChange()
        {
            int pageSize = 50;
            Devices = Enumerable.Range(1, pageSize).Select(i => _currentPage * pageSize + i).Select(i => "Device " + i.ToString()).ToList();
            _currentPage++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Devices)));
        }
    }
}
