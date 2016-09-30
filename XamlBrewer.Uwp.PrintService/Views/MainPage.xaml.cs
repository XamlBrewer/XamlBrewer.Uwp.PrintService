using Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlBrewer.Uwp.PrintService.ViewModels;

namespace XamlBrewer.Uwp.PrintService
{
    public sealed partial class MainPage : Page
    {
        private MenuItem resetItem;

        public MainPage()
        {
            this.InitializeComponent();

            resetItem = new MenuItem()
            {
                Glyph = Symbol.Preview,
                Text = "Print",
                Command = (DataContext as MainPageViewModel).PrintCommand
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (this.DataContext as ViewModelBase).Menu.Add(resetItem);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (this.DataContext as ViewModelBase).Menu.Remove(resetItem);
            base.OnNavigatedFrom(e);
        }
    }
}
