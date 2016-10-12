using Mvvm;
using Mvvm.Services.Printing;
using System.Diagnostics.Tracing;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Mvvm.Services;

namespace XamlBrewer.Uwp.PrintService
{
    public sealed partial class MainPage
    {
        private readonly PrintServiceProvider _printServiceProvider = new PrintServiceProvider();
        private readonly MenuItem _printItem;

        public MainPage()
        {
            InitializeComponent();

            var printCommand = new DelegateCommand(Print_Executed);
            _printItem = new MenuItem()
            {
                Glyph = Symbol.Page,
                Text = "Print",
                Command = printCommand
            };

            _printServiceProvider.StatusChanged += PrintServiceProvider_StatusChanged;
        }

        private void PrintServiceProvider_StatusChanged(object sender, PrintServiceEventArgs e)
        {
            switch (e.Level)
            {
                case EventLevel.Informational:
                    Log.Info(e.Message);
                    break;
                default:
                    Log.Error(e.Message);
                    Toast.Show(e.Message, "ms-appx:///Assets/Toasts/Printer.png");
                    break;
            }
        }

        private void Print_Executed()
        {
            _printServiceProvider.RegisterForPrinting(this, typeof(MainReport), DataContext);
            _printServiceProvider.Print();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (DataContext as ViewModelBase)?.Menu.Add(_printItem);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (DataContext as ViewModelBase)?.Menu.Remove(_printItem);
            _printServiceProvider.UnregisterForPrinting();
            base.OnNavigatedFrom(e);
        }
    }
}
