using System;

namespace Mvvm.Services.Printing
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;

    public sealed partial class PrintPage
    {
        public PrintPage()
        {
            InitializeComponent();
        }

        public PrintPage(RichTextBlockOverflow textLinkContainer)
            : this()
        {
            if (textLinkContainer == null) throw new ArgumentNullException(nameof(textLinkContainer));
            textLinkContainer.OverflowContentTarget = continuationPageLinkedContainer;
        }

        internal void AddContent(Paragraph block)
        {
            textContent.Blocks.Add(block);
        }
    }
}