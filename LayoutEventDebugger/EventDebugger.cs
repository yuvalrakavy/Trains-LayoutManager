using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.CommonUI;

#nullable enable
#pragma warning disable IDE0051, IDE0060

namespace LayoutEventDebugger {
    public enum TitleFormat {
        ShowEventName,
        ShowHandlerObject
    };

    /// <summary>
    /// Facilities to debug events. Subscription and event excerciser, subscrition view, event trace
    /// </summary>
    /// 
    [LayoutModule("Event Debugger")]
    public class EventDebugger : LayoutObject, ILayoutModuleSetup {
        public LayoutEventSubscription? ActiveSubscription;
        public string FiredEventName = "debug-event";
        public LayoutObject? Sender = new LayoutObject();
        public string? EventXmlInfo = null;

        private SubscriptionView? subscriptionView = null;
        private EventTrace? eventTrace = null;

        public EventDebugger() {
            XmlDocument.LoadXml("<Subscriber />");

            Sender.XmlInfo.XmlDocument.LoadXml("<EventSender />");
        }

        public LayoutModule? Module { set; get; }

        public void CancelActiveSubscription() {
            if (ActiveSubscription != null)
                EventManager.Subscriptions.Remove(ActiveSubscription);
            this.ActiveSubscription = null;
        }

        public void SetActiveSubscription(LayoutEventSubscription newSubscription) {
            if (this.ActiveSubscription != null)
                this.CancelActiveSubscription();

            this.ActiveSubscription = newSubscription;
            var method = this.GetType().GetMethod("OnDebuggedEvent", BindingFlags.Instance | BindingFlags.NonPublic);

            if (method != null) {
                ActiveSubscription.SetMethod(this, method);
                EventManager.Subscriptions.Add(ActiveSubscription);
            }
        }

        public void FireEvent(string eventName) {
            FiredEventName = eventName;

            var theEvent = new LayoutEvent(eventName, Sender);
            EventManager.Event(theEvent);
        }

        [LayoutEvent("tools-menu-open-request")]
        private void OnToolsMenuOpenRequest(LayoutEvent e) {
            var toolsMenu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            toolsMenu.Items.Add(new LayoutMenuItem("Event &Debugger", null, new EventHandler(this.OnEventDebuggerClick)));
            toolsMenu.Items.Add(new LayoutMenuItem("&View Subscriptions", null, new EventHandler(this.OnViewSubscriptionsClick)));
            toolsMenu.Items.Add(new LayoutMenuItem("&Trace events", null, new EventHandler(this.OnEventTraceClick)));
        }

        private void OnEventDebuggerClick(object? sender, EventArgs e) {
            EventDebuggerDialog eventDebuggerDialog = new EventDebuggerDialog(this);

            eventDebuggerDialog.ShowDialog();
        }

        private void OnViewSubscriptionsClick(object? sender, EventArgs e) {
            if (subscriptionView == null) {
                subscriptionView = new SubscriptionView();
                subscriptionView.Show();
            }
            else
                subscriptionView.Activate();
        }

        private void OnEventTraceClick(object? sender, EventArgs e) {
            if (eventTrace == null) {
                eventTrace = new EventTrace();
                eventTrace.Show();
            }
            else
                eventTrace.Activate();
        }

        [LayoutEvent("subscription-view-closed")]
        private void SubscriptionViewClosed(LayoutEvent e) {
            subscriptionView = null;
        }

        [LayoutEvent("event-trace-closed")]
        private void EventTraceClosed(LayoutEvent e) {
            eventTrace = null;
        }

        private void OnDebuggedEvent(LayoutEvent e) {
            MessageBox.Show("Event was intercepted by event debugger subscription", "Event Debugger", MessageBoxButtons.OK);
        }

        static public void AddSubscription(TreeNode parent, LayoutEventSubscriptionBase subscription, TitleFormat titleFormat) {
            var subscriptionNode = titleFormat switch
            {
                TitleFormat.ShowEventName => new TreeNode($"Event: {subscription.EventName ?? "<any>"}"),
                TitleFormat.ShowHandlerObject => new TreeNode($"Instance of {subscription.TargetObject?.GetType().FullName ?? "<static>"}"),
                _ => throw new NotImplementedException()
            };

            parent.Nodes.Add(subscriptionNode);

            subscriptionNode.Nodes.Add(new TreeNode($"Handler: {subscription.MethodName}"));

            if (subscription.Order != 0)
                subscriptionNode.Nodes.Add(new TreeNode($"Subscription order: {subscription.Order}"));

            if (subscription.SenderType != null)
                subscriptionNode.Nodes.Add(new TreeNode($"Sender type is {subscription.SenderType.FullName}"));

            if (subscription.EventType != null)
                subscriptionNode.Nodes.Add(new TreeNode($"Event type is {subscription.EventType.FullName}"));

            if (subscription.InfoType != null)
                subscriptionNode.Nodes.Add(new TreeNode($"Info type is {subscription.InfoType.FullName}"));

            if (subscription.IfSender != null) {
                TreeNode ifSenderNode = new TreeNode($"If sender XML matches: {subscription.IfSender}");

                subscriptionNode.Nodes.Add(ifSenderNode);
                if (subscription.NonExpandedIfSender.Contains('`'))
                    ifSenderNode.Nodes.Add(new TreeNode($"Non expanded: {subscription.NonExpandedIfSender}"));
            }

            if (subscription.IfEvent != null) {
                TreeNode IfEventNode = new TreeNode($"If event XML matches: {subscription.IfEvent}");

                subscriptionNode.Nodes.Add(IfEventNode);
                if (subscription.NonExpandedIfEvent.Contains('`'))
                    IfEventNode.Nodes.Add(new TreeNode($"Non expanded: {subscription.NonExpandedIfEvent}"));
            }

            if (subscription.IfInfo != null) {
                TreeNode IfInfoNode = new TreeNode("If Info XML matches: " + subscription.IfInfo);

                subscriptionNode.Nodes.Add(IfInfoNode);
                if (subscription.NonExpandedIfInfo.Contains('`'))
                    IfInfoNode.Nodes.Add(new TreeNode($"Non expanded: {subscription.NonExpandedIfInfo}"));
            }
        }
    }
}
