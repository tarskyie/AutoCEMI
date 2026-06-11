using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TabItemViewModel> tabs = new();

        [ObservableProperty]
        private TabItemViewModel? selectedTab;

        [RelayCommand]
        private void AddTab()
        {
            Tabs.Add(new TabItemViewModel("New Tab"));
            SelectedTab = Tabs.Last();
        }

        [RelayCommand]
        private void CloseTab(TabItemViewModel tab)
        {
            Tabs.Remove(tab);
        }
    }
}
