namespace Mvvm.Services.Printing
{
    using System;

    public class PrintServiceEventArgs : EventArgs
    {
        public PrintServiceEventArgs()
        { }

        public PrintServiceEventArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The message from the print service.
        /// </summary>
        public string Message { get; set; }
    }
}