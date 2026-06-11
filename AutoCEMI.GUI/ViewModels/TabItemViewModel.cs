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
        public string Header { get; }

        public TabItemViewModel(string header)
        {
            Header = header;
        }
    }
}
