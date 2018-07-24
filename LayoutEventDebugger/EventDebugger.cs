using System;
using System.Windows.Forms;
using System.Reflection;

using LayoutManager;

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
	public class EventDebugger : LayoutObject, ILayoutModuleSetup
	{
		LayoutModule		module;

		public LayoutEventSubscription	Subscription = new LayoutEventSubscription();
		public LayoutObject				Sender = new LayoutObject();
		public LayoutEvent				DebugEvent;
		public bool						SubscriptionActive = false;

		private SubscriptionView		subscriptionView = null;
		private EventTrace				eventTrace = null;

		public EventDebugger()
		{
			XmlDocument.LoadXml("<Subscriber />");

			Sender.XmlInfo.XmlDocument.LoadXml("<EventSender />");
			DebugEvent = new LayoutEvent(Sender, "debug-event");
			Subscription.SetMethod(this, this.GetType().GetMethod("OnDebuggedEvent", BindingFlags.Instance|BindingFlags.NonPublic));
		}

		public LayoutModule Module {
			set {
				module = value;
			}

			get {
				return module;
			}
		}

		[LayoutEvent("tools-menu-open-request")]
		private void OnToolsMenuOpenRequest(LayoutEvent e) {
			Menu toolsMenu = (Menu)e.Info;

			toolsMenu.MenuItems.Add(new MenuItem("Event &Debugger", new EventHandler(this.OnEventDebuggerClick)));
			toolsMenu.MenuItems.Add(new MenuItem("&View Subscriptions", new EventHandler(this.OnViewSubscriptionsClick)));
			toolsMenu.MenuItems.Add(new MenuItem("&Trace events", new EventHandler(this.OnEventTraceClick)));
		}

		void OnEventDebuggerClick(Object sender, EventArgs e) {
			EventDebuggerDialog	eventDebuggerDialog = new EventDebuggerDialog(this);

			eventDebuggerDialog.ShowDialog();
		}

		void OnViewSubscriptionsClick(Object sender, EventArgs e) {
			if(subscriptionView == null) {
				subscriptionView = new SubscriptionView();
				subscriptionView.Show();
			}
			else
				subscriptionView.Activate();

		}

		void OnEventTraceClick(Object sender, EventArgs e) {
			if(eventTrace == null) {
				eventTrace = new EventTrace();
				eventTrace.Show();
			}
			else
				eventTrace.Activate();
		}

		[LayoutEvent("subscription-view-closed")]
		void SubscriptionViewClosed(LayoutEvent e) {
			subscriptionView = null;
		}

		[LayoutEvent("event-trace-closed")]
		void EventTraceClosed(LayoutEvent e) {
			eventTrace = null;
		}

		void OnDebuggedEvent(LayoutEvent e) {
			MessageBox.Show("Event was intercepted by event debugger subscription", "Event Debugger", MessageBoxButtons.OK);
		}

		static public void AddSubscription(TreeNode parent, LayoutEventSubscriptionBase subscription, TitleFormat titleFormat) {
			TreeNode	subscriptionNode = null;

			switch(titleFormat) {

				case TitleFormat.ShowEventName:
					subscriptionNode = new TreeNode("Event: " + (subscription.EventName == null ? "<any>" : subscription.EventName));
					break;

				case TitleFormat.ShowHandlerObject:
					subscriptionNode = new TreeNode("Instance of " + subscription.TargetObject.GetType().FullName);
					break;
			}

			parent.Nodes.Add(subscriptionNode);

			subscriptionNode.Nodes.Add(new TreeNode("Handler: " + subscription.MethodName));
			
			if(subscription.Order != 0)
				subscriptionNode.Nodes.Add(new TreeNode("Subscription order: " + subscription.Order));

			if(subscription.SenderType != null)
				subscriptionNode.Nodes.Add(new TreeNode("Sender type is " + subscription.SenderType.FullName));

			if(subscription.EventType != null)
				subscriptionNode.Nodes.Add(new TreeNode("Event type is " + subscription.EventType.FullName));

			if(subscription.InfoType != null)
				subscriptionNode.Nodes.Add(new TreeNode("Info type is " + subscription.InfoType.FullName));

			if(subscription.IfSender != null) {
				TreeNode	ifSenderNode = new TreeNode("If sender XML matches: " + subscription.IfSender);

				subscriptionNode.Nodes.Add(ifSenderNode);
				if(subscription.NonExpandedIfSender.IndexOf('`') >= 0)
					ifSenderNode.Nodes.Add(new TreeNode("Non expanded: " + subscription.NonExpandedIfSender));
			}

			if(subscription.IfEvent != null) {
				TreeNode	IfEventNode = new TreeNode("If event XML matches: " + subscription.IfEvent);

				subscriptionNode.Nodes.Add(IfEventNode);
				if(subscription.NonExpandedIfEvent.IndexOf('`') >= 0)
					IfEventNode.Nodes.Add(new TreeNode("Non expanded: " + subscription.NonExpandedIfEvent));
			}

			if(subscription.IfInfo != null) {
				TreeNode	IfInfoNode = new TreeNode("If Info XML matches: " + subscription.IfInfo);

				subscriptionNode.Nodes.Add(IfInfoNode);
				if(subscription.NonExpandedIfInfo.IndexOf('`') >= 0)
					IfInfoNode.Nodes.Add(new TreeNode("Non expanded: " + subscription.NonExpandedIfInfo));
			}

		}

	}
}
