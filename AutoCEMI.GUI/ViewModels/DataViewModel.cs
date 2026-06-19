using AutoCEMI.Models;
using AutoCEMI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class DataViewModel : ObservableObject
    {
        [ObservableProperty]
        private DataService dataService = new DataService();

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [RelayCommand]
        private void RemoveMod(Mod mod)
        {
            DataService.RemoveMod(mod.Name);
        }

        [RelayCommand]
        private void RemovePort(SourcePort port)
        {
            DataService.RemovePort(port.Name);
        }

        [RelayCommand]
        private void RemoveIwad(Iwad iwad)
        {
            DataService.RemoveIwad(iwad.Name);
        }
    }
}
