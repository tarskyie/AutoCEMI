using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using System;
using Microsoft.Windows.Storage.Pickers;
using System.Threading.Tasks;
using Microsoft.UI;
using WinRT.Interop;

namespace AutoCEMI.GUI.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new();
        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            ViewModel.CloseTabCommand.Execute(args.Item as GameProfileViewModel);
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            ViewModel.AddTabCommand.Execute(null);
        }

        private void Data_MenuBar_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindowInstance.Navigate(typeof(DataPage));
        }

        private async void About_MenuBar_Click(object sender, RoutedEventArgs e)
        {
            await App.MainWindowInstance.DisplayAboutDialog();
        }

        private void New_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddTabCommand.Execute(null);
        }

        private async void Open_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await OpenFileWithPicker();
        }

        private async Task OpenFileWithPicker()
        {
            var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(App.MainWindowInstance));
            var openPicker = new FileOpenPicker(windowId);
            openPicker.FileTypeFilter.Add(".json");

            // Open the picker
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                ViewModel.ReadAndOpenJsonProfileCommand.Execute(file.Path);
            }
        }
    }
}
