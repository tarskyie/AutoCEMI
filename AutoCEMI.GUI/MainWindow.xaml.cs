using AutoCEMI.GUI.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace AutoCEMI.GUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
            }
            MainPage_Frame.Navigate(typeof(Views.MainPage));
        }

        public void Navigate(Type type)
        {
            MainPage_Frame.Navigate(type);
        }

        public void GoBack()
        {
            MainPage_Frame.GoBack();
        }
    }
}
