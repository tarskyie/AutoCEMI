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
            DataContext = new ViewModels.MainViewModel();
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            ViewModel.CloseTabCommand.Execute(args.Item as GameProfileViewModel);
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            ViewModel.AddTabCommand.Execute(null);
        }
    }
}
