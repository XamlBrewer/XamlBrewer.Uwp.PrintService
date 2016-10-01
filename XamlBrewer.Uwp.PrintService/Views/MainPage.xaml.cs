using Mvvm;
using Mvvm.Services.Printing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
        private PrintServiceProvider printServiceProvider = new PrintServiceProvider();
        private MenuItem printItem;
        private DelegateCommand printCommand;

        public MainPage()
        {
            this.InitializeComponent();

            printCommand = new DelegateCommand(Print_Executed);
            printItem = new MenuItem()
            {
                Glyph = Symbol.Page,
                Text = "Print",
                Command = printCommand
            };

            this.Loaded += this.MainPage_Loaded;
        }

        private void Print_Executed()
        {
            printServiceProvider.Print();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (this.DataContext as ViewModelBase).Menu.Add(printItem);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (this.DataContext as ViewModelBase).Menu.Remove(printItem);
            this.printServiceProvider.UnregisterForPrinting();
            base.OnNavigatedFrom(e);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.printServiceProvider.RegisterForPrinting(this, typeof(MainReport), this.DataContext);
        }
    }
}
