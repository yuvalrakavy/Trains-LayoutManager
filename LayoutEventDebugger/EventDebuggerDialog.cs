using LayoutManager;
using System;
using System.Windows.Forms;

#nullable enable
namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for EventDebuggerDialog.
    /// </summary>
    public partial class EventDebuggerDialog : Form {
        private readonly EventDebugger ed;

        public EventDebuggerDialog(EventDebugger ed) {
            this.ed = ed;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (ed.ActiveSubscription != null) {
                textBoxEventName.Text = ed.ActiveSubscription.EventName;

                if (ed.ActiveSubscription.SenderType != null)
                    textBoxEventType.Text = ed.ActiveSubscription.SenderType.FullName;
                else
                    textBoxEventType.Text = "";

                if (ed.ActiveSubscription.EventType != null)
                    textBoxEventType.Text = ed.ActiveSubscription.EventType.FullName;
                else
                    textBoxEventType.Text = "";

                textBoxIfSender.Text = ed.ActiveSubscription.NonExpandedIfSender;
                textBoxIfEvent.Text = ed.ActiveSubscription.NonExpandedIfEvent;
            }

            textBoxEvent.Text = ed.FiredEventName;

            var editedEvent = new LayoutEvent(ed.FiredEventName, sender: ed.Sender, xmlDocument: ed.EventXmlInfo);

            xmlInfoEditorSender.XmlInfo = ed.Sender?.XmlInfo;
            xmlInfoEditorEvent.XmlInfo = editedEvent.XmlInfo;
            xmlInfoEditorSubscriber.XmlInfo = ed.XmlInfo;

            UpdateDialog();
        }

        private void UpdateDialog() {
            labelSubscriptionStatus!.Text = $"Subscription is {(ed.ActiveSubscription != null ? "active" : "not active")}";
        }

        private LayoutEventSubscription? GetUpdattedSubscription() {
            try {
                Type? senderType;
                Type? eventType;
                string? ifSender;
                string? ifEvent;

                if (textBoxSenderType!.Text.Trim() == "")
                    senderType = null;
                else {
                    senderType = Type.GetType(textBoxSenderType.Text);

                    if (senderType == null) {
                        textBoxSenderType.Focus();
                        throw new ArgumentException($"No such type: { textBoxSenderType.Text} Invalid sender type");
                    }
                }

                if (textBoxEventType!.Text.Trim() == "")
                    eventType = null;
                else {
                    eventType = Type.GetType(textBoxEventType.Text);

                    if (eventType == null) {
                        textBoxEventType.Focus();
                        throw new ArgumentException($"No such type: { textBoxSenderType.Text} Invalid event type");
                    }
                }

                ifSender = string.IsNullOrWhiteSpace(textBoxIfSender!.Text) ? null : textBoxIfSender.Text;
                ifEvent = string.IsNullOrWhiteSpace(textBoxIfEvent!.Text) ? null : textBoxIfEvent.Text;

                return new LayoutEventSubscription(textBoxEventName!.Text) {
                    SenderType = senderType,
                    EventType = eventType,
                    IfSender = ifSender,
                    IfEvent = ifEvent,
                };
            }
            catch (ArgumentException error) {
                MessageBox.Show(this, error.Message, "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ButtonSubscribe_Click(object? sender, System.EventArgs e) {
            ed.CancelActiveSubscription();

            var newSubscription = this.GetUpdattedSubscription();
            if (newSubscription != null)
                ed.SetActiveSubscription(newSubscription);

            UpdateDialog();
        }

        private void ButtonUnsubscribe_Click(object? sender, System.EventArgs e) {
            ed.CancelActiveSubscription();

            UpdateDialog();
        }

        private void ButtonSendEvent_Click(object? sender, System.EventArgs e) {
            ed.FiredEventName = textBoxEvent!.Text;
            ed.EventXmlInfo = xmlInfoEditorEvent.XmlInfo?.DocumentElement?.InnerXml;
            LayoutEvent firedEvent = new LayoutEvent(ed.FiredEventName, sender: ed.Sender, xmlDocument: ed.EventXmlInfo);
            EventManager.Event(firedEvent);
        }
    }
}
