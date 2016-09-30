namespace Mvvm.Services.Printing
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;

    public sealed partial class PrintPage : Page
    {
        public PrintPage()
        {
            this.InitializeComponent();
        }

        public PrintPage(RichTextBlockOverflow textLinkContainer)
            : this()
        {
            textLinkContainer.OverflowContentTarget = continuationPageLinkedContainer;
        }

        internal void AddContent(Paragraph block)
        {
            this.textContent.Blocks.Add(block);
        }
    }
}