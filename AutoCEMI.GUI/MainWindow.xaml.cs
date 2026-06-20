using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoCEMI.GUI
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel? mainViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
            }
            MainPage_Frame.Navigate(typeof(Views.MainPage));
            AppWindow.Closing += AppWindow_Closing;
        }

        private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            if (mainViewModel == null)
                return;
            await mainViewModel.SaveState();
        }

        public void Navigate(Type type)
        {
            MainPage_Frame.Navigate(type);
        }

        public void GoBack()
        {
            MainPage_Frame.GoBack();
        }

        public async Task DisplayAboutDialog()
        {
            ContentDialog dialog = new ContentDialog();

            StackPanel stackPanel = new StackPanel();
            TextBlock appTitleTextBlock = new TextBlock()
            {
                Text = "Autonomous Configuration & Execution Management Interface (AutoCEMI)"
            };
            TextBlock versionTextBlock = new TextBlock()
            {
                Text = $"v. {Assembly.GetExecutingAssembly().GetName().Version}"
            };
            stackPanel.Children.Add(appTitleTextBlock);
            stackPanel.Children.Add(versionTextBlock);

            dialog.XamlRoot = RootGrid.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "About";
            dialog.SecondaryButtonText = "Close";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = stackPanel;

            var result = await dialog.ShowAsync();
        }
    }
}
