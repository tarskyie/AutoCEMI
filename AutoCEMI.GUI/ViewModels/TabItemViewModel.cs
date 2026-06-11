using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class TabItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string header;

        public TabItemViewModel(string header)
        {
            Header = header;
        }
    }
}
