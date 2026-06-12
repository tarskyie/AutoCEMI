using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AutoCEMI.GUI.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new();
        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            ViewModel.CloseTabCommand.Execute(args.Item as GameProfileViewModel);
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            ViewModel.AddTabCommand.Execute(null);
        }

        private void ProfileFlyoutData_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindowInstance.Navigate(typeof(DataPage));
        }
    }
}
