using System;
using MethodDispatcher;

namespace LayoutManager {
    public enum ExceptionMessageType {
        Message, Warning, Error
    }

    public class LayoutException : Exception {
        public LayoutException() {
        }

        public LayoutException(String message) : base(message) {
        }

        public LayoutException(object? subject, string message) : base(message) {
            this.Subject = subject;
        }

        public LayoutException(object? subject, string message, Exception inner) : base(message, inner) {
            this.Subject = subject;
        }

        /// <summary>
        /// The subject of this exception, this can be either a ModelComponent or a LayoutSelection or null
        /// </summary>
        public object? Subject { get; set; }

        /// <summary>
        /// The event to generate for reporting this exception. The default is add-error, but it can
        /// also be add-warning or add-message
        /// </summary>
        public virtual ExceptionMessageType DefaultMessageType => ExceptionMessageType.Error;

        /// <summary>
        /// Send to the message viewer the message associated with this exception. Use the default
        /// error level (message, warning or error) that was designated by the exception creator
        /// </summary>
        virtual public void Report() {
            switch(DefaultMessageType) {
                case ExceptionMessageType.Message: ReportMessage(); break;
                case ExceptionMessageType.Warning: ReportWarning(); break;
                case ExceptionMessageType.Error: ReportError(); break;
            }
        }

        /// <summary>
        /// Send to the message viewer the message associated with this exception. 
        /// Report the message as an error
        /// </summary>
        virtual public void ReportError() => Dispatch.Call.AddError(Message, Subject);

        /// <summary>
        /// Send to the message viewer the message associated with this exception. 
        /// Report the message as a warning
        /// </summary>
        virtual public void ReportWarning() => Dispatch.Call.AddWarning(Message, Subject);

        /// <summary>
        /// Send to the message viewer the message associated with this exception. 
        /// Report the message as a message
        /// </summary>
        virtual public void ReportMessage() => Dispatch.Call.AddMessage(Message, Subject);
    }

    public class FileParseException : Exception {
        public FileParseException(string message) : base(message) {
        }
    }

    public static class Ensure {
        public static T NotNull<T>(object? v, string name) where T : class {
            if (v == null)
                throw new ArgumentNullException(name);
            return (T)v;
        }

        public static T NotNull<T>(object? v) where T: class {
            if (v == null)
                throw new ArgumentNullException(nameof(v));
            return (T)v;
        }

        public static T ValueNotNull<T>(object? v, string name) where T : struct {
            if (v == null)
                throw new ArgumentNullException(name);
            return (T)v;
        }

        public static T ValueNotNull<T>(object? v) where T : struct {
            if (v == null)
                throw new ArgumentNullException(nameof(v));
            return (T)v;
        }

        public static (TSender, TInfo) NotNull<TSender, TInfo>(LayoutEvent e) where TSender : class where TInfo : class {
            return (Ensure.NotNull<TSender>(e.Sender), Ensure.NotNull<TInfo>(e.Info));
        }

    }
}
