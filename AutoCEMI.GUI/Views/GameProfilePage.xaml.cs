using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace AutoCEMI.GUI.Views
{
    public sealed partial class GameProfilePage : UserControl
    {
        public GameProfileViewModel ViewModel { get; } = new();
        
        public GameProfilePage()
        {
            InitializeComponent();
        }

        private async void AddModButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox modNameBox = new()
            {
                PlaceholderText = "Mod name"
            };

            ContentDialog dialog = new()
            {
                Title = "Add Mod",
                Content = modNameBox,
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string modName = modNameBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(modName))
                {
                    if (DataContext is GameProfileViewModel vm)
                    {
                        vm.AddModCommand.Execute(modName);
                    }
                }
            }
        }
    }
}
