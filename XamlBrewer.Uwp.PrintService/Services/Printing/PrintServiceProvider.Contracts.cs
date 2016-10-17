using System.Diagnostics.Tracing;

namespace Mvvm.Services.Printing
{
    using System;
    using Windows.Graphics.Printing;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Printing;

    /// <summary>
    /// Windows printing contract event handlers.
    /// </summary>
    public partial class PrintServiceProvider
    {
        /// <summary>
        /// Marker interface for document source
        /// </summary>
        private IPrintDocumentSource _printDocumentSource;

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
                            OnStatusChanged("Sorry, failed to print.", EventLevel.Error);
                        });
                    }
                };

                sourceRequested.SetSource(_printDocumentSource);
            });
        }

        /// <summary>
        /// Event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        {
            // PreparePrintContent();

            // Clear the cache of preview pages
            _printPreviewPages.Clear();
            _pageNumber = 0;

            // Clear the printing root.
            PrintingRoot.Children.Clear();

            // Get the page description to determine how big the page is
            PrintPageDescription pageDescription = e.PrintTaskOptions.GetPageDescription(0);

            var lastOverflow = AddOnePrintPreviewPage(null, pageDescription);
            while (lastOverflow.HasOverflowContent && lastOverflow.Visibility == Visibility.Visible)
            {
                lastOverflow = AddOnePrintPreviewPage(lastOverflow, pageDescription);
            }

            // Report the number of preview pages created
            var printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPageCount(_printPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// Event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page.
        /// </summary>
        private void PrintDocument_GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            var printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPage(e.PageNumber, _printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// Event handler for PrintDocument.AddPages.
        /// </summary>
        private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        {
            var printDoc = (PrintDocument)sender;

            // Add each preview page to the print document.
            foreach (var previewPage in _printPreviewPages)
            {
                printDoc.AddPage(previewPage);
            }

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }
    }
}