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
        private PrintPage _firstPrintPage;

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

            if (!PrintManager.IsSupported())
            {
                OnStatusChanged("Sorry, printing is not supported on this device.", EventLevel.Error);
            }

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

            var printManager = PrintManager.GetForCurrentView();
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
            switch (e.Severity)
            {
                case EventLevel.Critical:
                    StatusChanged?.Invoke(this, e);
                    break;
                case EventLevel.Error:
                    StatusChanged?.Invoke(this, e);
                    break;
                case EventLevel.Informational:
#if DEBUG
                    StatusChanged?.Invoke(this, e);
#endif
                    break;
                case EventLevel.LogAlways:
#if DEBUG
                    StatusChanged?.Invoke(this, e);
#endif
                    break;
                case EventLevel.Verbose:
#if DEBUG
                    StatusChanged?.Invoke(this, e);
#endif
                    break;
                case EventLevel.Warning:
                    StatusChanged?.Invoke(this, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
        /// Creates a print preview page and adds it to the internal list.
        /// </summary>
        private RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastOverflowAdded, PrintPageDescription printPageDescription)
        {
            PrintPage printPage;

            // Check if this is the first page.
            if (lastOverflowAdded == null)
            {
                printPage = _firstPrintPage;
            }
            else
            {
                // Flow content from previous pages.
                printPage = new PrintPage(lastOverflowAdded);

                // Remove the duplicate OverflowContentTarget.
                printPage.TextContent.OverflowContentTarget = null;
            }

            // Set page size.
            ApplyPrintPageDescription(printPageDescription, printPage);

            // Add title.
            var titleTextBlock = (TextBlock)printPage.FindName("title");
            if (titleTextBlock != null)
            {
                titleTextBlock.Text = Title;
            }

            // Add page number
            _pageNumber += 1;
            var pageNumberTextBlock = (TextBlock)printPage.FindName("pageNumber");
            if (pageNumberTextBlock != null)
            {
                pageNumberTextBlock.Text = string.Format("- {0} -", _pageNumber);
            }

            // Add the page to the page preview collection
            _printPreviewPages.Add(printPage);

            // The link container for text overflowing in this page
            return printPage.TextOverflow;
        }

        /// <summary>
        /// Applies height and width of the printer page.
        /// </summary>
        private void ApplyPrintPageDescription(PrintPageDescription printPageDescription, PrintPage printPage)
        {
            // Set paper size
            printPage.Width = printPageDescription.PageSize.Width;
            printPage.Height = printPageDescription.PageSize.Height;

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect.
            var marginWidth = Math.Max(
                printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width,
                printPageDescription.PageSize.Width * _horizontalPrintMargin * 2);
            var marginHeight = Math.Max(
                printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height,
                printPageDescription.PageSize.Height * _verticalPrintMargin * 2);

            // Set-up the printable area on the paper.
            printPage.PrintableArea.Width = printPage.Width - marginWidth;
            printPage.PrintableArea.Height = printPage.Height - marginHeight;
            OnStatusChanged("Printable area: width = " + printPage.PrintableArea.Width + ", height = " + printPage.PrintableArea.Height);

            // Add the page to the printing root which is part of the visual tree.
            // Force it to go through layout so that the overflow correctly distribute the content inside them.
            PrintingRoot.Children.Add(printPage);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
        }

        /// <summary>
        /// Prepare print content and send it to the Printing Root.
        /// </summary>
        private void PreparePrintContent()
        {
            // Create and populate print page.
            var userPrintPage = Activator.CreateInstance(_printPageType) as Page;
            if (userPrintPage == null)
            {
                OnStatusChanged("Configuration error: print page type is not a Page subclass", EventLevel.Error);
                return;
            }

            // Apply DataContext.
            userPrintPage.DataContext = DataContext;

            // Create print template page and fill invisible textblock with empty paragraph.
            // This will push all 'real' content into the overflow.
            _firstPrintPage = new PrintPage();
            _firstPrintPage.AddContent(new Paragraph());

            // Flatten content from user print page to a list of paragraphs, and move these to the print template.
            var userPrintPageContent = userPrintPage.Content as RichTextBlock;
            if (userPrintPageContent == null)
            {
                OnStatusChanged("Configuration error: print page's main panel is not a RichTextBlock.", EventLevel.Error);
                return;
            }

            while (userPrintPageContent.Blocks.Count > 0)
            {
                var paragraph = userPrintPageContent.Blocks.First() as Paragraph;
                userPrintPageContent.Blocks.Remove(paragraph);

                var container = paragraph.Inlines[0] as InlineUIContainer;
                if (container != null)
                {
                    var itemsControl = container.Child as ItemsControl;
                    if (itemsControl?.Items != null)
                    {
                        // Render the paragraph (only to read the ItemsSource).
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
                            _firstPrintPage.AddContent(itemParagraph);
                        }
                    }
                    else
                    {
                        // Place the paragraph in a new textblock, and measure it.
                        var actualHeight = Render(paragraph);

                        // Apply line height to trigger overflow.
                        paragraph.LineHeight = actualHeight;

                        _firstPrintPage.AddContent(paragraph);
                    }
                }
                else
                {
                    _firstPrintPage.AddContent(paragraph);
                }
            }

            OnStatusChanged("Prepared " + _firstPrintPage.TextContent.Blocks.Count + " paragraphs.");

            // Send it to the printing root.
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(_firstPrintPage);
        }

        // Renders a single paragraph.
        private double Render(Paragraph paragraph)
        {
            var currentHeight = paragraph.LineHeight;
            var blockToMeasure = new RichTextBlock();
            blockToMeasure.Blocks.Add(paragraph);
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(blockToMeasure);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
            blockToMeasure.Blocks.Clear();
            var newHeight = blockToMeasure.ActualHeight;
            OnStatusChanged(string.Format("Rendered paragraph height moved from {0} to {1}.", currentHeight, newHeight));

            return newHeight;
        }
    }
}