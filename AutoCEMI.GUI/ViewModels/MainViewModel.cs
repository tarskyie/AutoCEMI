using AutoCEMI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<GameProfileViewModel> tabs = new();

        [ObservableProperty]
        private GameProfileViewModel? selectedTab;

        [RelayCommand]
        private void AddTab()
        {
            Tabs.Add(new GameProfileViewModel());
            SelectedTab = Tabs.Last();
        }

        [RelayCommand]
        private void CloseTab(GameProfileViewModel tab)
        {
            Tabs.Remove(tab);
        }

        [RelayCommand]
        private void ReadAndOpenJsonProfile(string path)
        {
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);
                GameProfile? gameProfile = new();
                gameProfile = JsonSerializer.Deserialize<GameProfile>(jsonContent);

                Tabs.Add(new GameProfileViewModel() { Profile = gameProfile ?? new GameProfile(), ProfileLocationPath = path });
            }
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void CallUndo()
        {
            (SelectedTab as GameProfileViewModel)?.UndoCommand.Execute(this);
        }

        [RelayCommand(CanExecute = nameof(CanRedo))]
        private void CallRedo()
        {
            (SelectedTab as GameProfileViewModel)?.RedoCommand.Execute(this);
        }

        private bool CanUndo()
        {
            if (SelectedTab is GameProfileViewModel)
            {
                return SelectedTab.CanUndo();
            }
            return false;
        }

        private bool CanRedo()
        {
            if (SelectedTab is GameProfileViewModel)
            {
                return SelectedTab.CanRedo();
            }
            return false;
        }
    }
}
