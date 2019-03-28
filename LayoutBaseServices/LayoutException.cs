using System;

#nullable enable
namespace LayoutManager {

    public class LayoutException : Exception {
        object? subject;

        public LayoutException() {
        }

        public LayoutException(String message) : base(message) {
        }

        public LayoutException(object subject, String message) : base(message) {
            this.subject = subject;
        }

        public LayoutException(object subject, String message, Exception inner) : base(message, inner) {
            this.subject = subject;
        }

        /// <summary>
        /// The subject of this exception, this can be either a ModelComponent or a LayoutSelection or null
        /// </summary>
        public object? Subject {
            get {
                return subject;
            }

            set {
                subject = value;
            }
        }

        /// <summary>
        /// The event to generate for reporting this exception. The default is add-error, but it can
        /// also be add-warning or add-message
        /// </summary>
        public virtual String DefaultMessageType => "error";

        virtual protected void Generate(String messageType) {
            String messageEventName = "add-" + messageType;

            EventManager.Event(new LayoutEvent(messageEventName, Subject, Message));
        }

        /// <summary>
        /// Send to the message viewer the message associated with this exception. Use the default
        /// error level (message, warning or error) that was designated by the exception creator
        /// </summary>
        virtual public void Report() {
            Generate(DefaultMessageType);
        }

        /// <summary>
        /// Send to the message viewer the message associated with this exception. 
        /// Report the message as an error
        /// </summary>
        virtual public void ReportError() {
            Generate("error");
        }

        /// <summary>
        /// Send to the message viewer the message associated with this exception. 
        /// Report the message as a warning
        /// </summary>
        virtual public void ReportWarning() {
            Generate("warning");
        }

        /// <summary>
        /// Send to the message viewer the message associated with this exception. 
        /// Report the message as a message
        /// </summary>
        virtual public void ReportMessage() {
            Generate("message");
        }
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
    }
}
