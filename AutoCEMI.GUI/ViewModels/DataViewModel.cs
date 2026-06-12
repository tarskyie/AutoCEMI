using AutoCEMI.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class DataViewModel : ObservableObject
    {
        [ObservableProperty]
        private DataService dataService = new DataService();

        [ObservableProperty]
        private string statusMessage;

    }
}
