using AutoCEMI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            if (Tabs.Count == 1)
            {
                _ = File.WriteAllTextAsync($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AutoCEMI\\tabs.json", "{}");
            }
            Tabs.Remove(tab);
            _ = SaveState();
        }

        [RelayCommand]
        private void ReadAndOpenJsonProfile(string path)
        {
            _ = OpenProfileFromPath(path);
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

        public void SwitchToNextTab()
        {
            if (SelectedTab == null) return;
            int TabIndex = (Tabs.IndexOf(SelectedTab) + 1) % Tabs.Count();
            SelectedTab = Tabs[TabIndex];
        }

        public async Task SaveState()
        {
            if (SelectedTab == null) return;
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

        public async Task LoadState()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 0)
            {
                foreach (string arg in commandLineArgs)
                {
                    await OpenProfileFromPath(arg);
                }
            }

            try
            {
                var state = JsonSerializer.Deserialize<WorkspaceState>(File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AutoCEMI\\tabs.json"));
                if (state != null)
                {
                    foreach (var t in state.Tabs)
                    {
                        if (t.Profile == null) continue;
                        Tabs.Add(new GameProfileViewModel { Profile = t.Profile });
                    }
                    SelectedTab = Tabs[state.SelectedTabIndex];
                }
            }
            catch
            {
                if (Tabs.Count == 0)
                {
                    Tabs.Add(new GameProfileViewModel());
                    SelectedTab = Tabs[0];
                }
            }
        }

        public async Task OpenProfileFromPath(string path)
        {
            if (!File.Exists(path)) return;
            GameProfile? profile;
            try
            {
                profile = JsonSerializer.Deserialize<GameProfile>(File.ReadAllText(path));
            }
            catch { return; }
            if (profile != null)
            {
                var vm = new GameProfileViewModel() { Profile = profile, ProfileLocationPath = path };
                Tabs.Add(vm);
                SelectedTab = Tabs[Tabs.Count - 1];
            }
        }

        public MainViewModel()
        {
            _ = LoadState();
        }
    }

    public class WorkspaceState
    {
        public List<TabState> Tabs { get; set; } = [];
        public int SelectedTabIndex { get; set; }
    }

    public class TabState
    {
        public GameProfile? Profile { get; set; }
    }
}
