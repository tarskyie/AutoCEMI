using AutoCEMI.GUI.Services;
using AutoCEMI.GUI.ViewModels;
using AutoCEMI.Models;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
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

            _ = SleepSometimeAndAssignViewmodelToWindow();

            ViewModel.PropertyChanged += (sender, e) => { 
                if (ViewModel.Tabs.Count == 0 && App.MainWindowInstance != null)
                {
                    App.MainWindowInstance.Close();
                }
            };
        }
        private async Task SleepSometimeAndAssignViewmodelToWindow()
        {
            await Task.Delay(1000);
            App.MainWindowInstance.mainViewModel = ViewModel;
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

        private void CloseTab_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseTabCommand.Execute(Root_TabView.SelectedItem as GameProfileViewModel);
        }

        private async void Save_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await serviceToWriteGameProfiles.SaveProfile(Root_TabView.SelectedItem as GameProfileViewModel ?? new GameProfileViewModel());
        }

        private async void SaveAs_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await serviceToWriteGameProfiles.SaveProfileAsNew(Root_TabView.SelectedItem as GameProfileViewModel ?? new GameProfileViewModel());
        }

        private void Run_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            (Root_TabView.SelectedItem as GameProfileViewModel ?? new GameProfileViewModel()).ExecuteProfileCommand.Execute(this);
        }

        private async void EditExecParameters_Click(object sender, RoutedEventArgs e)
        {
            if (Root_TabView.SelectedItem is GameProfileViewModel gameProfileViewModel)
            {
                StackPanel stackPanel = new StackPanel()
                {
                    Spacing = 8
                };
                ComboBox mapsComboBox = new ComboBox()
                {
                    ItemsSource = new ObservableCollection<string> (gameProfileViewModel.GetMaps()),
                    Header = "Map",
                    SelectedItem = gameProfileViewModel.ExecutionOptions.Map,
                };
                Button resetMapButton = new Button() {
                    Content = "Reset map selection"
                };
                resetMapButton.Click += (sender, e) => mapsComboBox.SelectedItem = null;
                Slider skillSlider = new Slider()
                {
                    Header = "Skill",
                    Minimum = -1,
                    Maximum = 4,
                    StepFrequency = 1,
                    Value = gameProfileViewModel.ExecutionOptions.Difficulty
                };
                
                stackPanel.Children.Add(mapsComboBox);
                stackPanel.Children.Add(resetMapButton);
                stackPanel.Children.Add(skillSlider);

                ContentDialog contentDialog = new ContentDialog() {
                    Title = "Edit execution parameters",
                    Content = stackPanel,
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Ok"
                };
                
                ContentDialogResult result = await contentDialog.ShowAsync();

                gameProfileViewModel.ExecutionOptions.Map = mapsComboBox.SelectedItem as string ?? string.Empty;
                gameProfileViewModel.ExecutionOptions.Difficulty = (int)skillSlider.Value;
            }
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Menu && menuBar != null) // Alt key
            {
                menuBar.Focus(FocusState.Keyboard);
                e.Handled = true;
            }

            var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
            bool isCtrlPressed = (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            if (isCtrlPressed && e.Key == VirtualKey.Tab)
            {
                ViewModel.SwitchToNextTab();
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
