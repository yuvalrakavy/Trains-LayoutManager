using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for EventDebuggerDialog.
    /// </summary>
    public class EventDebuggerDialog : Form {
		private TabControl tabControl1;
		private TabPage tabPageSubscription;
		private TabPage tabPageSender;
		private TabPage tabPageEvent;
		private LayoutManager.CommonUI.Controls.XmlInfoEditor xmlInfoEditorSender;
		private LayoutManager.CommonUI.Controls.XmlInfoEditor xmlInfoEditorEvent;
		private Label labelEventName;
		private TextBox textBoxEventName;
		private Label labelSenderType;
		private TextBox textBoxSenderType;
		private TextBox textBoxEventType;
		private Label labelEventType;
		private TextBox textBoxIfSender;
		private Label labelIfSender;
		private TextBox textBoxIfEvent;
		private Label labelIfEvent;
		private Button buttonSubscribe;
		private Button buttonUnsubscribe;
		private Label labelSubscriptionStatus;
		private Button buttonSendEvent;
		private Button buttonClose;
		private TabPage tabPageSubscriber;
		private LayoutManager.CommonUI.Controls.XmlInfoEditor xmlInfoEditorSubscriber;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private Label labelEvent;
		private TextBox textBoxEvent;

		EventDebugger	ed;

		public EventDebuggerDialog(EventDebugger ed)
		{
			this.ed = ed;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			textBoxEventName.Text = ed.Subscription.EventName;

			if(ed.Subscription.SenderType != null)
				textBoxEventType.Text = ed.Subscription.SenderType.FullName;
			else
				textBoxEventType.Text = "";

			if(ed.Subscription.EventType != null)
				textBoxEventType.Text = ed.Subscription.EventType.FullName;
			else
				textBoxEventType.Text = "";

			textBoxIfSender.Text = ed.Subscription.NonExpandedIfSender;
			textBoxIfEvent.Text = ed.Subscription.NonExpandedIfEvent;
			textBoxEvent.Text = ed.DebugEvent.EventName;

			xmlInfoEditorSender.XmlInfo = ed.Sender.XmlInfo;
			xmlInfoEditorEvent.XmlInfo = ed.DebugEvent.XmlInfo;
			xmlInfoEditorSubscriber.XmlInfo = ed.XmlInfo;

			updateDialog();
		}

		void updateDialog() {
			if(ed.SubscriptionActive)
				labelSubscriptionStatus.Text = "Subscription is active";
			else
				labelSubscriptionStatus.Text = "Subscription is not active";
		}

		bool updateSubscription() {
			bool	ok = true;

			ed.Subscription.EventName = textBoxEventName.Text;

			if(textBoxSenderType.Text.Trim() == "")
				ed.Subscription.SenderType = null;
			else {
				Type	senderType = Type.GetType(textBoxSenderType.Text);

				if(senderType == null) {
					MessageBox.Show(this, "No such type: " + textBoxSenderType.Text, "Invalid sender type", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBoxSenderType.Focus();
					ok = false;
				}
				else
					ed.Subscription.SenderType = senderType;
			}

			if(textBoxEventType.Text.Trim() == "")
				ed.Subscription.EventType = null;
			else {
				Type	eventType = Type.GetType(textBoxEventType.Text);

				if(eventType == null) {
					MessageBox.Show(this, "No such type: " + textBoxEventType.Text, "Invalid sender type", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBoxEventType.Focus();
					ok = false;
				}
				else
					ed.Subscription.EventType = eventType;
			}

			if(textBoxIfSender.Text.Trim() == "")
				ed.Subscription.IfSender = null;
			else
				ed.Subscription.IfSender = textBoxIfSender.Text;

			if(textBoxIfEvent.Text.Trim() == "")
				ed.Subscription.IfEvent = null;
			else
				ed.Subscription.IfEvent = textBoxIfEvent.Text;

			return ok;
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
			this.xmlInfoEditorEvent = new LayoutManager.CommonUI.Controls.XmlInfoEditor();
			this.tabPageSender = new TabPage();
			this.xmlInfoEditorSender = new LayoutManager.CommonUI.Controls.XmlInfoEditor();
			this.textBoxEventType = new TextBox();
			this.textBoxEventName = new TextBox();
			this.labelIfEvent = new Label();
			this.textBoxSenderType = new TextBox();
			this.labelEventType = new Label();
			this.textBoxIfEvent = new TextBox();
			this.labelEventName = new Label();
			this.buttonSendEvent = new Button();
			this.tabPageEvent = new TabPage();
			this.buttonClose = new Button();
			this.buttonUnsubscribe = new Button();
			this.buttonSubscribe = new Button();
			this.labelSubscriptionStatus = new Label();
			this.tabControl1 = new TabControl();
			this.tabPageSubscription = new TabPage();
			this.textBoxIfSender = new TextBox();
			this.labelIfSender = new Label();
			this.labelSenderType = new Label();
			this.tabPageSubscriber = new TabPage();
			this.xmlInfoEditorSubscriber = new LayoutManager.CommonUI.Controls.XmlInfoEditor();
			this.labelEvent = new Label();
			this.textBoxEvent = new TextBox();
			this.tabPageSender.SuspendLayout();
			this.tabPageEvent.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageSubscription.SuspendLayout();
			this.tabPageSubscriber.SuspendLayout();
			this.SuspendLayout();
			// 
			// xmlInfoEditorEvent
			// 
			this.xmlInfoEditorEvent.Location = new System.Drawing.Point(0, 40);
			this.xmlInfoEditorEvent.Name = "xmlInfoEditorEvent";
			this.xmlInfoEditorEvent.Size = new System.Drawing.Size(368, 208);
			this.xmlInfoEditorEvent.TabIndex = 0;
			this.xmlInfoEditorEvent.XmlInfo = null;
			// 
			// tabPageSender
			// 
			this.tabPageSender.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.xmlInfoEditorSender});
			this.tabPageSender.Location = new System.Drawing.Point(4, 22);
			this.tabPageSender.Name = "tabPageSender";
			this.tabPageSender.Size = new System.Drawing.Size(368, 246);
			this.tabPageSender.TabIndex = 1;
			this.tabPageSender.Text = "Sender";
			// 
			// xmlInfoEditorSender
			// 
			this.xmlInfoEditorSender.Name = "xmlInfoEditorSender";
			this.xmlInfoEditorSender.Size = new System.Drawing.Size(368, 246);
			this.xmlInfoEditorSender.TabIndex = 0;
			this.xmlInfoEditorSender.XmlInfo = null;
			// 
			// textBoxEventType
			// 
			this.textBoxEventType.Location = new System.Drawing.Point(88, 78);
			this.textBoxEventType.Name = "textBoxEventType";
			this.textBoxEventType.Size = new System.Drawing.Size(160, 20);
			this.textBoxEventType.TabIndex = 5;
			this.textBoxEventType.Text = "";
			// 
			// textBoxEventName
			// 
			this.textBoxEventName.Location = new System.Drawing.Point(88, 16);
			this.textBoxEventName.Name = "textBoxEventName";
			this.textBoxEventName.Size = new System.Drawing.Size(160, 20);
			this.textBoxEventName.TabIndex = 1;
			this.textBoxEventName.Text = "";
			// 
			// labelIfEvent
			// 
			this.labelIfEvent.Location = new System.Drawing.Point(8, 152);
			this.labelIfEvent.Name = "labelIfEvent";
			this.labelIfEvent.Size = new System.Drawing.Size(112, 16);
			this.labelIfEvent.TabIndex = 8;
			this.labelIfEvent.Text = "Event filter (XPath):";
			// 
			// textBoxSenderType
			// 
			this.textBoxSenderType.Location = new System.Drawing.Point(88, 46);
			this.textBoxSenderType.Name = "textBoxSenderType";
			this.textBoxSenderType.Size = new System.Drawing.Size(160, 20);
			this.textBoxSenderType.TabIndex = 3;
			this.textBoxSenderType.Text = "";
			// 
			// labelEventType
			// 
			this.labelEventType.Location = new System.Drawing.Point(8, 78);
			this.labelEventType.Name = "labelEventType";
			this.labelEventType.Size = new System.Drawing.Size(72, 16);
			this.labelEventType.TabIndex = 4;
			this.labelEventType.Text = "Event type:";
			// 
			// textBoxIfEvent
			// 
			this.textBoxIfEvent.Location = new System.Drawing.Point(120, 150);
			this.textBoxIfEvent.Name = "textBoxIfEvent";
			this.textBoxIfEvent.Size = new System.Drawing.Size(216, 20);
			this.textBoxIfEvent.TabIndex = 9;
			this.textBoxIfEvent.Text = "";
			// 
			// labelEventName
			// 
			this.labelEventName.Location = new System.Drawing.Point(8, 18);
			this.labelEventName.Name = "labelEventName";
			this.labelEventName.Size = new System.Drawing.Size(100, 16);
			this.labelEventName.TabIndex = 0;
			this.labelEventName.Text = "Event name:";
			// 
			// buttonSendEvent
			// 
			this.buttonSendEvent.Location = new System.Drawing.Point(8, 288);
			this.buttonSendEvent.Name = "buttonSendEvent";
			this.buttonSendEvent.Size = new System.Drawing.Size(80, 23);
			this.buttonSendEvent.TabIndex = 13;
			this.buttonSendEvent.Text = "Send &Event";
			this.buttonSendEvent.Click += new System.EventHandler(this.buttonSendEvent_Click);
			// 
			// tabPageEvent
			// 
			this.tabPageEvent.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.textBoxEvent,
																					   this.labelEvent,
																					   this.xmlInfoEditorEvent});
			this.tabPageEvent.Location = new System.Drawing.Point(4, 22);
			this.tabPageEvent.Name = "tabPageEvent";
			this.tabPageEvent.Size = new System.Drawing.Size(368, 246);
			this.tabPageEvent.TabIndex = 2;
			this.tabPageEvent.Text = "Event";
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(304, 288);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 14;
			this.buttonClose.Text = "Close";
			// 
			// buttonUnsubscribe
			// 
			this.buttonUnsubscribe.Location = new System.Drawing.Point(96, 184);
			this.buttonUnsubscribe.Name = "buttonUnsubscribe";
			this.buttonUnsubscribe.Size = new System.Drawing.Size(80, 23);
			this.buttonUnsubscribe.TabIndex = 11;
			this.buttonUnsubscribe.Text = "&Unsubscribe";
			this.buttonUnsubscribe.Click += new System.EventHandler(this.buttonUnsubscribe_Click);
			// 
			// buttonSubscribe
			// 
			this.buttonSubscribe.Location = new System.Drawing.Point(8, 184);
			this.buttonSubscribe.Name = "buttonSubscribe";
			this.buttonSubscribe.Size = new System.Drawing.Size(80, 23);
			this.buttonSubscribe.TabIndex = 10;
			this.buttonSubscribe.Text = "&Subscribe";
			this.buttonSubscribe.Click += new System.EventHandler(this.buttonSubscribe_Click);
			// 
			// labelSubscriptionStatus
			// 
			this.labelSubscriptionStatus.Location = new System.Drawing.Point(8, 216);
			this.labelSubscriptionStatus.Name = "labelSubscriptionStatus";
			this.labelSubscriptionStatus.Size = new System.Drawing.Size(256, 16);
			this.labelSubscriptionStatus.TabIndex = 12;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.tabPageSubscription,
																					  this.tabPageEvent,
																					  this.tabPageSender,
																					  this.tabPageSubscriber});
			this.tabControl1.Location = new System.Drawing.Point(8, 8);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(376, 272);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageSubscription
			// 
			this.tabPageSubscription.Controls.AddRange(new System.Windows.Forms.Control[] {
																							  this.labelSubscriptionStatus,
																							  this.buttonUnsubscribe,
																							  this.buttonSubscribe,
																							  this.textBoxIfEvent,
																							  this.labelIfEvent,
																							  this.textBoxIfSender,
																							  this.labelIfSender,
																							  this.textBoxEventType,
																							  this.labelEventType,
																							  this.textBoxSenderType,
																							  this.labelSenderType,
																							  this.textBoxEventName,
																							  this.labelEventName});
			this.tabPageSubscription.Location = new System.Drawing.Point(4, 22);
			this.tabPageSubscription.Name = "tabPageSubscription";
			this.tabPageSubscription.Size = new System.Drawing.Size(368, 246);
			this.tabPageSubscription.TabIndex = 0;
			this.tabPageSubscription.Text = "Subscription";
			// 
			// textBoxIfSender
			// 
			this.textBoxIfSender.Location = new System.Drawing.Point(120, 118);
			this.textBoxIfSender.Name = "textBoxIfSender";
			this.textBoxIfSender.Size = new System.Drawing.Size(216, 20);
			this.textBoxIfSender.TabIndex = 7;
			this.textBoxIfSender.Text = "";
			// 
			// labelIfSender
			// 
			this.labelIfSender.Location = new System.Drawing.Point(8, 120);
			this.labelIfSender.Name = "labelIfSender";
			this.labelIfSender.Size = new System.Drawing.Size(112, 16);
			this.labelIfSender.TabIndex = 6;
			this.labelIfSender.Text = "Sender filter (XPath):";
			// 
			// labelSenderType
			// 
			this.labelSenderType.Location = new System.Drawing.Point(8, 48);
			this.labelSenderType.Name = "labelSenderType";
			this.labelSenderType.Size = new System.Drawing.Size(72, 16);
			this.labelSenderType.TabIndex = 2;
			this.labelSenderType.Text = "Sender type:";
			// 
			// tabPageSubscriber
			// 
			this.tabPageSubscriber.Controls.AddRange(new System.Windows.Forms.Control[] {
																							this.xmlInfoEditorSubscriber});
			this.tabPageSubscriber.Location = new System.Drawing.Point(4, 22);
			this.tabPageSubscriber.Name = "tabPageSubscriber";
			this.tabPageSubscriber.Size = new System.Drawing.Size(368, 246);
			this.tabPageSubscriber.TabIndex = 3;
			this.tabPageSubscriber.Text = "Subscriber";
			// 
			// xmlInfoEditorSubscriber
			// 
			this.xmlInfoEditorSubscriber.Name = "xmlInfoEditorSubscriber";
			this.xmlInfoEditorSubscriber.Size = new System.Drawing.Size(368, 246);
			this.xmlInfoEditorSubscriber.TabIndex = 0;
			this.xmlInfoEditorSubscriber.XmlInfo = null;
			// 
			// labelEvent
			// 
			this.labelEvent.Location = new System.Drawing.Point(8, 8);
			this.labelEvent.Name = "labelEvent";
			this.labelEvent.Size = new System.Drawing.Size(40, 16);
			this.labelEvent.TabIndex = 1;
			this.labelEvent.Text = "Event:";
			// 
			// textBoxEvent
			// 
			this.textBoxEvent.Location = new System.Drawing.Point(56, 6);
			this.textBoxEvent.Name = "textBoxEvent";
			this.textBoxEvent.Size = new System.Drawing.Size(232, 20);
			this.textBoxEvent.TabIndex = 2;
			this.textBoxEvent.Text = "debug-event";
			// 
			// EventDebuggerDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 318);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonClose,
																		  this.buttonSendEvent,
																		  this.tabControl1});
			this.Name = "EventDebuggerDialog";
			this.Text = "Event Debugger";
			this.tabPageSender.ResumeLayout(false);
			this.tabPageEvent.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPageSubscription.ResumeLayout(false);
			this.tabPageSubscriber.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonSubscribe_Click(object sender, System.EventArgs e) {
			if(ed.SubscriptionActive)
				EventManager.Subscriptions.Remove(ed.Subscription);

			if(updateSubscription()) {
				EventManager.Subscriptions.Add(ed.Subscription);
				ed.SubscriptionActive = true;
			}

			updateDialog();
		}

		private void buttonUnsubscribe_Click(object sender, System.EventArgs e) {
			if(ed.SubscriptionActive)
				EventManager.Subscriptions.Remove(ed.Subscription);

			updateSubscription();
			ed.SubscriptionActive = false;

			updateDialog();
		}

		private void buttonSendEvent_Click(object sender, System.EventArgs e) {
			ed.DebugEvent.EventName = textBoxEvent.Text;
			EventManager.Event(ed.DebugEvent);
		}
	}
}
