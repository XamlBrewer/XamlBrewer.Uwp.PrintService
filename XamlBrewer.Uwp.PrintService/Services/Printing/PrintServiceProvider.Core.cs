using System.Diagnostics.Tracing;

namespace Mvvm.Services.Printing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Graphics.Printing;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Printing;

    public partial class PrintServiceProvider : Page
    {
        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        private PrintDocument _printDocument;

        /// <summary>
        /// The app page that calls the print service.
        /// </summary>
        private Page _callingPage;

        /// <summary>
        /// The type of print page.
        /// </summary>
        private Type _printPageType;

        /// <summary>
        /// The page number.
        /// </summary>
        private int _pageNumber;

        /// <summary>
        /// The percent of margin width, content is set at 90% of the page's width
        /// </summary>
        private double _horizontalPrintMargin = 0.05;

        /// <summary>
        /// The percent of margin height, content is set at 95% of the page's height
        /// </summary>
        private double _verticalPrintMargin = 0.025;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        private readonly List<UIElement> _printPreviewPages = new List<UIElement>();

        /// <summary>
        /// First page in the printing-content series.
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages".
        /// </summary>
        private PrintPage _firstPage;

        /// <summary>
        /// Expose information to the subscriber.
        /// </summary>
        /// <remarks>
        /// Alternatively, use interface or event aggregator, or other messaging.
        /// </remarks>
        public event EventHandler<PrintServiceEventArgs> StatusChanged;

        /// <summary>
        /// The title of the document, will be printed in the header.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///  Printing root canvas on the calling page.
        /// </summary>
        private Canvas PrintingRoot => _callingPage.FindName("printingRoot") as Canvas;

        /// <summary>
        /// Registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public void RegisterForPrinting(Page sourcePage, Type printPageType, object viewModel)
        {
            _callingPage = sourcePage;

            if (PrintingRoot == null)
            {
                OnStatusChanged("The calling page has no PrintingRoot Canvas.", EventLevel.Error);
                return;
            }

            _printPageType = printPageType;
            DataContext = viewModel;

            // Prep the content
            PreparePrintContent();

            // Create the PrintDocument.
            _printDocument = new PrintDocument();

            // Save the DocumentSource.
            _printDocumentSource = _printDocument.DocumentSource;

            // Add an event handler which creates preview pages.
            _printDocument.Paginate += PrintDocument_Paginate;

            // Add an event handler which provides a specified preview page.
            _printDocument.GetPreviewPage += PrintDocument_GetPrintPreviewPage;

            // Add an event handler which provides all final print pages.
            _printDocument.AddPages += PrintDocument_AddPages;

            // Create a PrintManager and add a handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();

            printMan.PrintTaskRequested -= PrintManager_PrintTaskRequested;
            printMan.PrintTaskRequested += PrintManager_PrintTaskRequested;
            OnStatusChanged("Registered successfully.");
        }

        /// <summary>
        /// Unregisters the app for printing with Windows.
        /// </summary>
        public void UnregisterForPrinting()
        {
            if (_printDocument == null)
            {
                return;
            }

            _printDocument.Paginate -= PrintDocument_Paginate;
            _printDocument.GetPreviewPage -= PrintDocument_GetPrintPreviewPage;
            _printDocument.AddPages -= PrintDocument_AddPages;

            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;

            PrintingRoot.Children.Clear();
        }

        /// <summary>
        /// Opens the print dialog programmatically.
        /// </summary>
        public async void Print()
        {
            // Notify Caller
            OnStatusChanged("Opening Print Dialog.");

            try
            {
                await PrintManager.ShowPrintUIAsync();
                OnStatusChanged("Print Dialog opened.");
            }
            catch (Exception)
            {
                // You get here if you didn't register for print.
                OnStatusChanged("Attempt to open Print Dialog without registering first.", EventLevel.Error);
            }
        }

        /// <summary>
        /// Raises the status changed event.
        /// </summary>
        protected virtual void OnStatusChanged(PrintServiceEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        protected virtual void OnStatusChanged(string message)
        {
            OnStatusChanged(new PrintServiceEventArgs(message));
        }

        protected virtual void OnStatusChanged(string message, EventLevel level)
        {
            OnStatusChanged(new PrintServiceEventArgs(message, level));
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="lastOverflowAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        private RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastOverflowAdded, PrintPageDescription printPageDescription)
        {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page;

            // Check if this is the first page
            if (lastOverflowAdded == null)
            {
                page = _firstPage;
            }
            else
            {
                // Flow content (text) from previous pages
                page = new PrintPage(lastOverflowAdded);

                // Remove the duplicate OverflowContentTarget.
                var textContent = ((RichTextBlock)page.FindName("textContent"));
                if (textContent != null)
                {
                    textContent.OverflowContentTarget = null;
                }
            }

            // Set page size.
            ApplyPrintPageDescription(printPageDescription, page);

            // Add title
            TextBlock titleTextBlock = (TextBlock)page.FindName("title");
            if (titleTextBlock != null)
            {
                titleTextBlock.Text = Title;
            }

            // Add page number
            _pageNumber += 1;
            TextBlock pageNumberTextBlock = (TextBlock)page.FindName("pageNumber");
            if (pageNumberTextBlock != null)
            {
                pageNumberTextBlock.Text = string.Format("- {0} -", _pageNumber);
            }

            // Add the page to the page preview collection
            _printPreviewPages.Add(page);

            // The link container for text overflowing in this page
            var textLink = (RichTextBlockOverflow)page.FindName("continuationPageLinkedContainer");
            return textLink;
        }

        /// <summary>
        /// Applies height and width of the printer page.
        /// </summary>
        private void ApplyPrintPageDescription(PrintPageDescription printPageDescription, FrameworkElement page)
        {
            // Set paper size
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            var printableArea = (Grid)page.FindName("printableArea");
            if (printableArea == null)
            {
                return;
            }

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            var marginWidth = Math.Max(
                printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width,
                printPageDescription.PageSize.Width * _horizontalPrintMargin * 2);
            var marginHeight = Math.Max(
                printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height,
                printPageDescription.PageSize.Height * _verticalPrintMargin * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = page.Width - marginWidth;
            printableArea.Height = page.Height - marginHeight;

            // Add the (newly created) page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintingRoot.Children.Add(page);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
        }

        /// <summary>
        /// Prepare print content and send it to the Printing Root.
        /// </summary>
        private void PreparePrintContent()
        {
            // Create and populate print page.
            var printPage = Activator.CreateInstance(_printPageType) as Page;
            if (printPage == null)
            {
                // Raise event
                // ...
                return;
            }

            printPage.DataContext = DataContext;

            // Create print template page and fill invisible textblock with empty paragraph.
            // This pushes all real content into the overflow.
            _firstPage = new PrintPage();
            _firstPage.AddContent(new Paragraph());

            // Move content from print page to print template - paragraph by paragraph.
            var printPageRtb = printPage.Content as RichTextBlock;
            if (printPageRtb == null)
            {
                // Raise event
                // ...
                return;
            }

            while (printPageRtb.Blocks.Count > 0)
            {
                var paragraph = printPageRtb.Blocks.First() as Paragraph;
                if (paragraph == null)
                {
                    // Raise event
                    // ...
                    return;
                }

                printPageRtb.Blocks.Remove(paragraph);

                var container = paragraph.Inlines[0] as InlineUIContainer;
                if (container != null)
                {
                    var itemsControl = container.Child as ItemsControl;
                    if (itemsControl?.Items != null)
                    {
                        // Render the paragraph, to read the ItemsSource
                        Render(paragraph);

                        foreach (var item in itemsControl.Items)
                        {
                            var x = itemsControl.ContainerFromItem(item) as ContentPresenter;
                            if (x?.ContentTemplate == null)
                            {
                                // Raise event
                                // ...
                                return;
                            }

                            Paragraph p = new Paragraph();
                            InlineUIContainer c = new InlineUIContainer();
                            var o = x.ContentTemplate.LoadContent() as UIElement;
                            var f = o as FrameworkElement;
                            if (f == null)
                            {
                                // Raise
                                // ...
                                return;
                            }

                            f.DataContext = item;
                            c.Child = o;
                            p.Inlines.Add(c);
                            p.LineHeight = Render(p);
                            _firstPage.AddContent(p);
                        }
                    }
                    else
                    {
                        // Place the paragraph in a new textblock, and measure it.
                        var actualHeight = Render(paragraph);

                        // Apply line height to trigger overflow.
                        paragraph.LineHeight = actualHeight;

                        _firstPage.AddContent(paragraph);
                    }
                }
                else
                {
                    _firstPage.AddContent(paragraph);
                }
            }

            // Send it to the printing root.
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(_firstPage);
        }

        // Renders a single paragraph.
        private double Render(Paragraph paragraph)
        {
            var measureRtb = new RichTextBlock();
            measureRtb.Blocks.Add(paragraph);
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(measureRtb);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
            measureRtb.Blocks.Clear();

            return measureRtb.ActualHeight;
        }
    }
}