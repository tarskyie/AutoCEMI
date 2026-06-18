using AutoCEMI.Models;
using AutoCEMI.Services;
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
            _ = SaveState();
        }

        [RelayCommand]
        private void CloseTab(GameProfileViewModel tab)
        {
            Tabs.Remove(tab);
            _ = SaveState();
        }

        [RelayCommand]
        private void ReadAndOpenJsonProfile(string path)
        {
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);
                GameProfile? gameProfile = new();
                try
                { gameProfile = JsonSerializer.Deserialize<GameProfile>(jsonContent); }
                catch { return; }
                Tabs.Add(new GameProfileViewModel() { Profile = gameProfile ?? new GameProfile(), ProfileLocationPath = path });
                _ = SaveState();
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

        public async Task SaveState()
        {
            var state = new WorkspaceState
            {
                SelectedTabIndex = Tabs.IndexOf(SelectedTab),
                Tabs = Tabs.Select(t => new TabState
                {
                    Profile = t.Profile,
                }).ToList()
            };

            var json = JsonSerializer.Serialize(state);
            await File.WriteAllTextAsync($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AutoCEMI\\tabs.json", json);
        }
    }

    public class WorkspaceState
    {
        public List<TabState> Tabs { get; set; } = [];
        public int SelectedTabIndex { get; set; }
    }

    public class TabState
    {
        public GameProfile Profile{ get; set; }
    }
}
