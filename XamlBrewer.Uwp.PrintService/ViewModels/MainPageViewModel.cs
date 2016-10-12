using Mvvm;
using System.Collections.Generic;
using XamlBrewer.Uwp.PrintService.DataAccessLayer;
using XamlBrewer.Uwp.PrintService.Models;

namespace XamlBrewer.Uwp.PrintService.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        public List<TypeModel> Types
        {
            get { return Dal.GetTypes(); }
        }

        public List<CharacterModel> Characters
        {
            get { return Dal.GetCharacters(); }
        }
    }
}
