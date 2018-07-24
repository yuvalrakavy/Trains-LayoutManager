using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for EventTrace.
    /// </summary>
    public class EventTrace : Form {
		private TreeView treeViewEventTrace;
		private Button buttonTraceState;
		private Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private Button buttonClear;

		public EventTrace()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			EventManager.AddObjectSubscriptions(this);
		}

		void StartTrace() {
			buttonTraceState.Text = "Stop trace";
			EventManager.Instance.TraceEvents = true;
		}

		void EndTrace() {
			buttonTraceState.Text = "Start trace";
			EventManager.Instance.TraceEvents = false;
		}

		[LayoutEvent("trace-event")]
		void TraceEvent(LayoutEvent eBase) {
			LayoutEventTraceEvent	e = (LayoutEventTraceEvent)eBase;
			LayoutEvent				theEvent = (LayoutEvent)e.Sender;

			TreeNode	eventNode = new TreeNode(theEvent.EventName);
			
			if(e.Scope != LayoutEventScope.MyProcess)
				eventNode.Nodes.Add(new TreeNode("Scope " + e.Scope));

			if(theEvent.Sender != null)
				eventNode.Nodes.Add(new TreeNode("Sender: " + theEvent.Sender.GetType().FullName));

			if(theEvent.Info != null)
				eventNode.Nodes.Add(new TreeNode("Info: (" + theEvent.Info.GetType().Name + ") = " + theEvent.Info.ToString()));

			if(theEvent.TargetType != null)
				eventNode.Nodes.Add(new TreeNode("Target type is " + theEvent.TargetType.FullName));

			if(theEvent.IfTarget != null)
				eventNode.Nodes.Add(new TreeNode("Target XML match: " + theEvent.IfTarget));

			if(e.ApplicableSubscriptions.Count > 0) {
				TreeNode	subscriptionsNode = new TreeNode("Applicable subscriptions");

				foreach(LayoutEventSubscriptionBase subscription in e.ApplicableSubscriptions)
					EventDebugger.AddSubscription(subscriptionsNode, subscription, TitleFormat.ShowHandlerObject);

				eventNode.Nodes.Add(subscriptionsNode);
			}

			treeViewEventTrace.Nodes.Add(eventNode);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeViewEventTrace = new TreeView();
			this.buttonTraceState = new Button();
			this.buttonClose = new Button();
			this.buttonClear = new Button();
			this.SuspendLayout();
			// 
			// treeViewEventTrace
			// 
			this.treeViewEventTrace.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.treeViewEventTrace.ImageIndex = -1;
			this.treeViewEventTrace.Location = new System.Drawing.Point(8, 8);
			this.treeViewEventTrace.Name = "treeViewEventTrace";
			this.treeViewEventTrace.SelectedImageIndex = -1;
			this.treeViewEventTrace.Size = new System.Drawing.Size(280, 344);
			this.treeViewEventTrace.TabIndex = 0;
			// 
			// buttonTraceState
			// 
			this.buttonTraceState.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonTraceState.Location = new System.Drawing.Point(8, 360);
			this.buttonTraceState.Name = "buttonTraceState";
			this.buttonTraceState.Size = new System.Drawing.Size(72, 24);
			this.buttonTraceState.TabIndex = 1;
			this.buttonTraceState.Text = "Start trace";
			this.buttonTraceState.Click += new System.EventHandler(this.buttonTraceState_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(216, 360);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 3;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// buttonClear
			// 
			this.buttonClear.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonClear.Location = new System.Drawing.Point(88, 360);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(75, 24);
			this.buttonClear.TabIndex = 2;
			this.buttonClear.Text = "Clear";
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// EventTrace
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(296, 389);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonClear,
																		  this.buttonClose,
																		  this.buttonTraceState,
																		  this.treeViewEventTrace});
			this.Name = "EventTrace";
			this.Text = "Event Trace";
			this.Closed += new System.EventHandler(this.EventTrace_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void EventTrace_Closed(object sender, System.EventArgs e) {
			EventManager.Instance.TraceEvents = false;
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
			EventManager.Event(new LayoutEvent(this, "event-trace-closed"));
		}

		private void buttonTraceState_Click(object sender, System.EventArgs e) {
			if(EventManager.Instance.TraceEvents)
				EndTrace();
			else
				StartTrace();
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}

		private void buttonClear_Click(object sender, System.EventArgs e) {
			treeViewEventTrace.Nodes.Clear();
		}
	}
}
