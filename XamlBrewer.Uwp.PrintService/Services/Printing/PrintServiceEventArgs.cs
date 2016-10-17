using System.Diagnostics.Tracing;

namespace Mvvm.Services.Printing
{
    using System;

    public class PrintServiceEventArgs : EventArgs
    {
        public PrintServiceEventArgs()
        { }

        public PrintServiceEventArgs(string message, EventLevel level)
        {
            Message = message;
            Severity = level;
        }

        public PrintServiceEventArgs(string message) : this(message, EventLevel.Informational)
        {
        }

        /// <summary>
        /// The message from the print service.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The severity level of the message.
        /// </summary>
        public EventLevel Severity { get; set; }
    }
}