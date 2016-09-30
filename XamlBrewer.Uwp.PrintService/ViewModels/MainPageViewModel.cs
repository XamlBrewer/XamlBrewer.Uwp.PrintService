using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XamlBrewer.Uwp.PrintService.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        private DelegateCommand _printCommand;

        public MainPageViewModel()
        {
            _printCommand = new DelegateCommand(Print_Executed);
        }

        public ICommand PrintCommand
        {
            get { return _printCommand; }
        }

        private void Print_Executed()
        { }
    }
}
