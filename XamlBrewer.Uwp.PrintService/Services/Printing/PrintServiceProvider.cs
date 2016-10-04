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

    public class PrintServiceProvider : Page
    {
        /// <summary>
        /// The app page that calls the print service.
        /// </summary>
        private Page callingPage;

        /// <summary>
        /// The type of print page.
        /// </summary>
        private Type printPageType;

        /// <summary>
        /// The page number.
        /// </summary>
        private int pageNumber;

        /// <summary>
        /// The percent of margin width, content is set at 90% of the page's width
        /// </summary>
        private double HorizontalPrintMargin = 0.05;

        /// <summary>
        /// The percent of margin height, content is set at 95% of the page's height
        /// </summary>
        private double VerticalPrintMargin = 0.025;

        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        private PrintDocument printDocument = null;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        private IPrintDocumentSource printDocumentSource = null;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        private List<UIElement> printPreviewPages = new List<UIElement>();

        /// <summary>
        /// First page in the printing-content series.
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages".
        /// </summary>
        private PrintPage firstPage;

        /// <summary>
        /// Expose information to the subscriber.
        /// </summary>
        /// <remarks>
        /// Alternatively, use interface or event aggregator, or other messaging.
        /// </remarks>
        public event EventHandler<PrintServiceEventArgs> StatusChanged;

        /// <summary>
        /// Print service provider.
        /// </summary>
        public PrintServiceProvider()
        {
        }

        /// <summary>
        /// The title of the document, will be printed in the header.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///  Printing root canvas on the calling page.
        /// </summary>
        private Canvas PrintingRoot
        {
            get
            {
                return this.callingPage.FindName("printingRoot") as Canvas;
            }
        }

        /// <summary>
        /// Registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public void RegisterForPrinting(Page sourcePage, Type printPageType, object viewModel)
        {
            this.callingPage = sourcePage;

            if (PrintingRoot == null)
            {
                this.OnStatusChanged(new PrintServiceEventArgs("The calling page has no PrintingRoot Canvas."));
                return;
            }

            this.printPageType = printPageType;
            this.DataContext = viewModel;

            // Prep the content
            this.PreparePrintContent();

            // Create the PrintDocument.
            printDocument = new PrintDocument();

            // Save the DocumentSource.
            printDocumentSource = printDocument.DocumentSource;

            // Add an event handler which creates preview pages.
            printDocument.Paginate += PrintDocument_Paginate;

            // Add an event handler which provides a specified preview page.
            printDocument.GetPreviewPage += PrintDocument_GetPrintPreviewPage;

            // Add an event handler which provides all final print pages.
            printDocument.AddPages += PrintDocument_AddPages;

            // Create a PrintManager and add a handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();

            printMan.PrintTaskRequested -= PrintManager_PrintTaskRequested;
            printMan.PrintTaskRequested += PrintManager_PrintTaskRequested;
            this.OnStatusChanged(new PrintServiceEventArgs("Registered successfully."));
        }

        /// <summary>
        /// Unregisters the app for printing with Windows.
        /// </summary>
        public void UnregisterForPrinting()
        {
            if (printDocument == null)
            {
                return;
            }

            printDocument.Paginate -= PrintDocument_Paginate;
            printDocument.GetPreviewPage -= PrintDocument_GetPrintPreviewPage;
            printDocument.AddPages -= PrintDocument_AddPages;

            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;

            PrintingRoot.Children.Clear();
        }

        /// <summary>
        /// Opens the print charm programmatically.
        /// </summary>
        public async void Print()
        {
            // Notify Caller
            this.OnStatusChanged(new PrintServiceEventArgs("Opening Print Dialog."));

            try
            {
                await PrintManager.ShowPrintUIAsync();
                this.OnStatusChanged(new PrintServiceEventArgs("")); // Print dialog open, but there's no close event handler
            }
            catch (Exception)
            {
                // you get here if you didn't register for print.
                this.OnStatusChanged(new PrintServiceEventArgs("Did you forget to register?"));
            }
        }

        /// <summary>
        /// Raises the status changed event.
        /// </summary>
        protected virtual void OnStatusChanged(PrintServiceEventArgs e)
        {
            this.StatusChanged?.Invoke(this, e);
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("XAML Brewer UWP Print Sample", sourceRequested =>
            {
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    // Notify the user when the print operation fails.
                    if (args.Completion == PrintTaskCompletion.Failed)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            this.OnStatusChanged(new PrintServiceEventArgs("Sorry, failed to print."));
                        });
                    }
                };

                sourceRequested.SetSource(printDocumentSource);
            });
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        {
            // Clear the cache of preview pages
            printPreviewPages.Clear();
            this.pageNumber = 0;

            // Clear the printing root of preview pages
            PrintingRoot.Children.Clear();

            // This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
            RichTextBlockOverflow lastRTBOOnPage;

            // Get the PrintTaskOptions
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);

            // Get the page description to deterimine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            // We know there is at least one page to be printed. passing null as the first parameter to
            // AddOnePrintPreviewPage tells the function to add the first page.
            lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);

            // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
            // page has extra content
            while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Report the number of preview pages created
            printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        private void PrintDocument_GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;

            printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < printPreviewPages.Count; i++)
            {
                // We should have all pages ready at this point...
                printDocument.AddPage(printPreviewPages[i]);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        private RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
        {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page;

            // The link container for text overflowing in this page
            RichTextBlockOverflow textLink;

            // Check if this is the first page ( no previous RichTextBlockOverflow)
            if (lastRTBOAdded == null)
            {
                // If this is the first page add the specific scenario content
                page = firstPage;
            }
            else
            {
                // Flow content (text) from previous pages
                page = new PrintPage(lastRTBOAdded);

                // Remove the duplicate OverflowContentTarget.
                ((RichTextBlock)page.FindName("textContent")).OverflowContentTarget = null;
            }

            // Set paper width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            Grid printableArea = (Grid)page.FindName("printableArea");

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * HorizontalPrintMargin * 2);
            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * VerticalPrintMargin * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = page.Width - marginWidth;
            printableArea.Height = page.Height - marginHeight;

            // Add the (newly created) page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintingRoot.Children.Add(page);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();

            // Find the last text container and see if the content is overflowing
            textLink = (RichTextBlockOverflow)page.FindName("continuationPageLinkedContainer");

            // Add title
            TextBlock titleTextBlock = (TextBlock)page.FindName("title");
            if (titleTextBlock != null)
            {
                titleTextBlock.Text = this.Title;
            }

            // Add page number
            this.pageNumber += 1;
            TextBlock pageNumberTextBlock = (TextBlock)page.FindName("pageNumber");
            if (pageNumberTextBlock != null)
            {
                pageNumberTextBlock.Text = string.Format("- {0} -", this.pageNumber);
            }

            // Add the page to the page preview collection
            printPreviewPages.Add(page);

            return textLink;
        }

        /// <summary>
        /// Prepare print content and send it to the Printing Root.
        /// </summary>
        private void PreparePrintContent()
        {
            // Create and populate print page.
            var printPage = Activator.CreateInstance(this.printPageType) as Page;
            printPage.DataContext = this.DataContext;

            // Create print template page and fill invisible textblock with empty paragraph.
            // This pushes all real content into the overflow.
            firstPage = new PrintPage();
            firstPage.AddContent(new Paragraph());

            // Move content from print page to print template - paragraph by paragraph.
            var printPageRtb = printPage.Content as RichTextBlock;
            while (printPageRtb.Blocks.Count > 0)
            {
                var paragraph = printPageRtb.Blocks.First() as Paragraph;
                printPageRtb.Blocks.Remove(paragraph);

                var container = paragraph.Inlines[0] as InlineUIContainer;
                if (container != null)
                {
                    var itemsControl = container.Child as ItemsControl;
                    if (itemsControl != null)
                    {
                        // Render the paragraph, to read the ItemsSource
                        this.Render(paragraph);

                        foreach (var item in itemsControl.Items)
                        {
                            var x = itemsControl.ContainerFromItem(item) as ContentPresenter;
                            Paragraph p = new Paragraph();
                            InlineUIContainer c = new InlineUIContainer();
                            var o = x.ContentTemplate.LoadContent() as UIElement;
                            (o as FrameworkElement).DataContext = item;
                            c.Child = o;
                            p.Inlines.Add(c);
                            p.LineHeight = this.Render(p);
                            firstPage.AddContent(p);
                        }
                    }
                    else
                    {
                        // Place the paragraph in a new textblock, and measure it.
                        double actualHeight = this.Render(paragraph);

                        // Apply line height to trigger overflow.
                        paragraph.LineHeight = actualHeight;

                        firstPage.AddContent(paragraph);
                    }
                }
                else
                {
                    firstPage.AddContent(paragraph);
                }
            };

            // Send it to the printing root.
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(firstPage);
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