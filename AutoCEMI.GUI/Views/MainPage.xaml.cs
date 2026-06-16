using AutoCEMI.GUI.Services;
using AutoCEMI.GUI.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using WinRT.Interop;

namespace AutoCEMI.GUI.Views
{
    public sealed partial class MainPage : Page
    {
        private ServiceToWriteGameProfiles serviceToWriteGameProfiles = new();
        private MenuBar? menuBar;
        public MainViewModel ViewModel { get; } = new();
        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            this.Content.KeyDown += OnKeyDown;
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

        private void Exit_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindowInstance.Close();
        }

        private async void Save_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await serviceToWriteGameProfiles.SaveProfile(Root_TabView.SelectedItem as GameProfileViewModel ?? new GameProfileViewModel());
        }

        private async void SaveAs_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await serviceToWriteGameProfiles.SaveProfileAsNew(Root_TabView.SelectedItem as GameProfileViewModel ?? new GameProfileViewModel());
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Menu && menuBar != null) // Alt key
            {
                menuBar.Focus(FocusState.Keyboard);
                e.Handled = true;
            }
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

        private void MainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            menuBar = (MenuBar)sender;
        }
    }
}
