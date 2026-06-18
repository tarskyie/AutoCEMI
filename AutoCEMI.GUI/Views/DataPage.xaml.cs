using AutoCEMI.GUI.Services;
using AutoCEMI.GUI.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using WinRT.Interop;

namespace AutoCEMI.GUI.Views
{
    public sealed partial class DataPage : Page
    {
        ValidatorsService validatorsService = new();
        public DataPage()
        {
            InitializeComponent();
            DataContext = new DataViewModel();
        }

        private void GoBack_Button_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindowInstance.GoBack();
        }

        private async Task AddItemDialog(string type)
        {
            if (type != "Source Port" && type != "IWAD" && type != "Mod")
            {
                return;
            }

            TextBox nameBox = new()
            {
                PlaceholderText = $"{type} name",
                Header = "Name"
            };

            Grid pathGrid = new()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            TextBox pathBox = new()
            {
                PlaceholderText = "Path to file",
                Header = "Path",
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            Button pathPickerButton = new()
            {
                Content = "Pick",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
            };

            pathPickerButton.Click += async (sender, e) => await PickPathForFile(pathBox);

            Grid.SetColumn(pathBox, 0);
            Grid.SetColumn(pathPickerButton, 1);

            pathGrid.Children.Add(pathBox);
            pathGrid.Children.Add(pathPickerButton);


            StackPanel stackPanel = new StackPanel()
            {
                Children = { nameBox, pathGrid },
                Spacing = 16
            };

            ContentDialog dialog = new()
            {
                Title = $"Add {type}",
                Content = stackPanel,
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string itemName = nameBox.Text.Trim();
                string itemPath = pathBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(itemName) && validatorsService.IsValidPath(itemPath))
                {
                    if (DataContext is DataViewModel vm)
                    {
                        // Execute command
                        switch (type)
                        {
                            case ("IWAD"):
                                vm.DataService.AddIwad(new Models.Iwad(itemName,itemPath));
                                break;
                            case ("Mod"):
                                vm.DataService.AddMod(new Models.Mod(itemName, itemPath));
                                break;
                            case ("Source Port"):
                                vm.DataService.AddPort(new Models.SourcePort(itemName,itemPath));
                                break;
                        }
                    }
                }
            }
        }

        private async Task PickPathForFile(TextBox textBox)
        {
            var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(App.MainWindowInstance));
            var openPicker = new FileOpenPicker(windowId);
            openPicker.FileTypeFilter.Add("*");

            // Open the picker
            var file = await openPicker.PickSingleFileAsync();

            textBox.Text = file.Path;
        }
        private async void AddIwads_Button_Click(object sender, RoutedEventArgs e) { await AddItemDialog("IWAD"); }
        private async void AddMods_Button_Click(object sender, RoutedEventArgs e) { await AddItemDialog("Mod"); }
        private async void AddSourcePorts_Button_Click(object sender, RoutedEventArgs e) { await AddItemDialog("Source Port"); }
    }
}
