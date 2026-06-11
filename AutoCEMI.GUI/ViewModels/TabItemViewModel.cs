using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCEMI.Models;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class TabItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string header;

        [ObservableProperty]
        private GameProfile profile = new();

        public TabItemViewModel(string header)
        {
            Header = header;
        }
    }
}
