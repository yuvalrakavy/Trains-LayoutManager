using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for EventTrace.
    /// </summary>
    public partial class EventTrace : Form {
        public EventTrace() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            EventManager.AddObjectSubscriptions(this);
        }

        private void StartTrace() {
            buttonTraceState.Text = "Stop trace";
            EventManager.Instance.TraceEvents = true;
        }

        private void EndTrace() {
            buttonTraceState.Text = "Start trace";
            EventManager.Instance.TraceEvents = false;
        }

        [LayoutEvent("trace-event")]
        private void TraceEvent(LayoutEvent eBase) {
            LayoutEventTraceEvent e = (LayoutEventTraceEvent)eBase;
            var theEvent = Ensure.NotNull<LayoutEvent>(e.Sender);

            TreeNode eventNode = new TreeNode(theEvent.EventName);

            if (e.Scope != LayoutEventScope.MyProcess)
                eventNode.Nodes.Add(new TreeNode("Scope " + e.Scope));

            if (theEvent.Sender != null)
                eventNode.Nodes.Add(new TreeNode("Sender: " + theEvent.Sender.GetType().FullName));

            if (theEvent.Info != null)
                eventNode.Nodes.Add(new TreeNode("Info: (" + theEvent.Info.GetType().Name + ") = " + theEvent.Info.ToString()));

            if (theEvent.TargetType != null)
                eventNode.Nodes.Add(new TreeNode("Target type is " + theEvent.TargetType.FullName));

            if (theEvent.IfTarget != null)
                eventNode.Nodes.Add(new TreeNode("Target XML match: " + theEvent.IfTarget));

            if (e.ApplicableSubscriptions.Count > 0) {
                TreeNode subscriptionsNode = new TreeNode("Applicable subscriptions");

                foreach (LayoutEventSubscriptionBase subscription in e.ApplicableSubscriptions)
                    EventDebugger.AddSubscription(subscriptionsNode, subscription, TitleFormat.ShowHandlerObject);

                eventNode.Nodes.Add(subscriptionsNode);
            }

            treeViewEventTrace.Nodes.Add(eventNode);
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

        private void EventTrace_Closed(object? sender, System.EventArgs e) {
            EventManager.Instance.TraceEvents = false;
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
            EventManager.Event(new LayoutEvent("event-trace-closed", this));
        }

        private void ButtonTraceState_Click(object? sender, System.EventArgs e) {
            if (EventManager.Instance.TraceEvents)
                EndTrace();
            else
                StartTrace();
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            this.Close();
        }

        private void ButtonClear_Click(object? sender, System.EventArgs e) {
            treeViewEventTrace.Nodes.Clear();
        }
    }
}
