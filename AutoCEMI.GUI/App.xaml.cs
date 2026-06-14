using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

namespace AutoCEMI.GUI
{
    public partial class App : Application
    {
        private Window? _window;
        public static MainWindow MainWindowInstance { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindowInstance = _window as MainWindow;
            _window.Activate();
        }
    }
}
