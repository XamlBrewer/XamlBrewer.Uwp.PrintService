using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace XamlBrewer.Uwp.PrintService.Models
{
    public class CharacterModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PrimaryComicTrait { get; set; }

        public string ImagePath { get; set; }

        public ImageSource Image
        {
            get
            {
                return new BitmapImage(new Uri("ms-appx:///" + this.ImagePath));
            }
        }
    }
}
