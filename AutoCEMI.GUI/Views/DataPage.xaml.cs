using AutoCEMI.GUI.Services;
using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;

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

            TextBox pathBox = new()
            {
                PlaceholderText = "Path to file",
                Header = "Path"
            };

            StackPanel stackPanel = new StackPanel()
            {
                Children = { nameBox, pathBox },
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
        private async void AddIwads_Button_Click(object sender, RoutedEventArgs e) { await AddItemDialog("IWAD"); }
        private async void AddMods_Button_Click(object sender, RoutedEventArgs e) { await AddItemDialog("Mod"); }
        private async void AddSourcePorts_Button_Click(object sender, RoutedEventArgs e) { await AddItemDialog("Source Port"); }
    }
}
