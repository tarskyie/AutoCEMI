using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AutoCEMI.GUI.Views
{
    public sealed partial class DataPage : Page
    {
        public DataPage()
        {
            InitializeComponent();
            DataContext = new DataViewModel();
        }

        private void GoBack_Button_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindowInstance.GoBack();
        }
    }
}
