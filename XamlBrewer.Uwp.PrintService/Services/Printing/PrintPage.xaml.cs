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
            textLinkContainer.OverflowContentTarget = textOverflow;
        }

        internal Grid PrintableArea => printableArea;

        internal RichTextBlock TextContent => textContent;

        internal RichTextBlockOverflow TextOverflow => textOverflow;

        internal void AddContent(Paragraph block)
        {
            textContent.Blocks.Add(block);
        }
    }
}