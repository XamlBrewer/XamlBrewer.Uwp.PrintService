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
        /// The Document that is sent to the printer.
        /// </summary>
        private PrintDocument _printDocument;

        /// <summary>
        /// The app page that called the print service.
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
        /// A list of UIElements used to store the print preview pages.
        /// </summary>
        private readonly List<UIElement> _printPreviewPages = new List<UIElement>();

        /// <summary>
        /// The first page in the printing-content series. It hosts all the rendered paragraphs.
        /// </summary>
        private PrintPage _firstPage;

        /// <summary>
        /// Exposes status information to the subscriber.
        /// </summary>
        public event EventHandler<PrintServiceEventArgs> StatusChanged;

        /// <summary>
        /// The title of the document. It will be printed in the header.
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

            // Prep the content.
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
                OnStatusChanged("Oops. You probably tried to open Print Dialog without registering first.", EventLevel.Error);
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
        /// Creates a print preview page and adds it to the internal cache.
        /// </summary>
        private RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastOverflowAdded, PrintPageDescription printPageDescription)
        {
            PrintPage page;

            // Check if this is the first page.
            if (lastOverflowAdded == null)
            {
                page = _firstPage;
            }
            else
            {
                // Flow content from previous pages.
                page = new PrintPage(lastOverflowAdded);

                // Remove the duplicate OverflowContentTarget.
                page.TextContent.OverflowContentTarget = null;
            }

            // Set page size.
            ApplyPrintPageDescription(printPageDescription, page);

            // Add title.
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
            return page.TextOverflow;
        }

        /// <summary>
        /// Applies height and width of the printer page.
        /// </summary>
        private void ApplyPrintPageDescription(PrintPageDescription printPageDescription, PrintPage page)
        {
            // Set paper size
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            var marginWidth = Math.Max(
                printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width,
                printPageDescription.PageSize.Width * _horizontalPrintMargin * 2);
            var marginHeight = Math.Max(
                printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height,
                printPageDescription.PageSize.Height * _verticalPrintMargin * 2);

            // Set-up the printable area on the paper.
            page.PrintableArea.Width = page.Width - marginWidth;
            page.PrintableArea.Height = page.Height - marginHeight;
            OnStatusChanged("Printable area: width = " + page.PrintableArea.Width + ", height = " + page.PrintableArea.Height);

            // Add the page to the printing root which is part of the visual tree.
            // Force it to go through layout so that the overflow correctly distribute the content inside them.
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
                OnStatusChanged("Configuration error: print page type is not a PrintPage subclass", EventLevel.Error);
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
                OnStatusChanged("Configuration error: print page's main panel is not a RichTextBlock.", EventLevel.Error);
                return;
            }

            while (printPageRtb.Blocks.Count > 0)
            {
                var paragraph = printPageRtb.Blocks.First() as Paragraph;
                printPageRtb.Blocks.Remove(paragraph);

                var container = paragraph.Inlines[0] as InlineUIContainer;
                if (container != null)
                {
                    var itemsControl = container.Child as ItemsControl;
                    if (itemsControl?.Items != null)
                    {
                        // Render the paragraph. Just to read the ItemsSource.
                        Render(paragraph);

                        // Transform each item to a paragraph, render separately, and add to the page.
                        foreach (var item in itemsControl.Items)
                        {
                            var itemParagraph = new Paragraph();
                            var inlineContainer = new InlineUIContainer();
                            var element = (itemsControl.ContainerFromItem(item) as ContentPresenter).ContentTemplate.LoadContent() as UIElement;
                            var frameworkElement = element as FrameworkElement;
                            frameworkElement.DataContext = item;
                            inlineContainer.Child = element;
                            itemParagraph.Inlines.Add(inlineContainer);
                            itemParagraph.LineHeight = Render(itemParagraph);
                            _firstPage.AddContent(itemParagraph);
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
            var blockToMeasure = new RichTextBlock();
            blockToMeasure.Blocks.Add(paragraph);
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(blockToMeasure);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
            blockToMeasure.Blocks.Clear();

            return blockToMeasure.ActualHeight;
        }
    }
}