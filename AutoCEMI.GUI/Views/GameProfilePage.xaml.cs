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
    public sealed partial class GameProfilePage : Page
    {
        ObservableCollection<string> loadOrder = new ObservableCollection<string>();
        public GameProfilePage()
        {
            InitializeComponent();
            LoadOrderUpdate();
        }

        private void LoadOrderUpdate()
        {
            loadOrder = DataContext is TabItemViewModel vm
                ? new ObservableCollection<string>(vm.Profile.Mods.LoadOrder)
                : new ObservableCollection<string>();

            LoadOrder_ItemsControl.ItemsSource = loadOrder;
        }

        private void AddMod(string name)
        {
            if (DataContext is ViewModels.TabItemViewModel vm) vm.Profile.Mods.LoadOrder.Add(name);
            LoadOrderUpdate();
        }
        private void RemoveMod(string name)
        {
            if (DataContext is ViewModels.TabItemViewModel vm) vm.Profile.Mods.LoadOrder.Remove(name);
            LoadOrderUpdate();
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
                    AddMod(modName);
                }
            }
        }

        private void RemoveModButton_Click(object sender, RoutedEventArgs e)
        {
            string modName = (string)((Button)sender).Tag;
            RemoveMod(modName);
        }
    }
}
