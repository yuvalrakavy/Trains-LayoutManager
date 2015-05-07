using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Model;
using LayoutManager.View;

namespace LayoutManager
{
	/// <summary>
	/// Summary description for MessageViewer.
	/// </summary>
	public class MessageViewer : System.Windows.Forms.UserControl
	{
		ListViewItem		lastMessageMarker = null;

		LayoutSelection		currentMessageSelection = null;
		LayoutSelection		currentComponentSelection = null;
		bool				clearOnNewMessage = false;

		static LayoutTraceSwitch	traceMessages = new LayoutTraceSwitch("Messages", "Trace errors, warning etc.");

		private ListView listViewMessages;
		private ColumnHeader columnHeaderMessage;
		private ColumnHeader columnHeaderArea;
		private Button buttonClose;
		private Button buttonPrevMessage;
		private Button buttonNextMessage;
		private Button buttonPreviousComponent;
		private Button buttonNextComponent;
		private ImageList imageListSeverity;
		private IContainer components;

		public MessageViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			lastMessageMarker = new ListViewItem();
			listViewMessages.Items.Add(lastMessageMarker);
		}

		public void Initialize() {
			EventManager.AddObjectSubscriptions(this);
		}

		[LayoutEvent("add-message")]
		void AddMessage(LayoutEvent e) {
			AddMessageItem(new MessageItem(MessageSeverity.Message, e.Sender, (String)e.Info));
			columnHeaderArea.Width = -1;
		}

		[LayoutEvent("add-warning")]
		void AddWarning(LayoutEvent e) {
			AddMessageItem(new MessageItem(MessageSeverity.Warning, e.Sender, (String)e.Info));
			columnHeaderArea.Width = -1;
		}

		[LayoutEvent("add-error")]
		void AddError(LayoutEvent e) {
			AddMessageItem(new MessageItem(MessageSeverity.Error, e.Sender, (String)e.Info));
		}

		void AddMessageItem(MessageItem item) {
			item.TraceMessage();

			if(clearOnNewMessage) {
				EventManager.Event(new LayoutEvent(this, "clear-messages"));
				clearOnNewMessage = false;
			}

			listViewMessages.Items.Insert(listViewMessages.Items.Count-1, item);
			columnHeaderArea.Width = -1;

			if(item.Severity != MessageSeverity.Message)
				EventManager.Event(new LayoutEvent(this, "show-messages"));

			if(listViewMessages.SelectedItems.Count == 1 && listViewMessages.SelectedItems[0] == lastMessageMarker)
				listViewMessages.EnsureVisible(lastMessageMarker.Index);

			UpdateButtons();
		}

		protected void UpdateButtons() {
			if(listViewMessages.SelectedIndices.Count == 0) {
				buttonPrevMessage.Enabled = false;
				buttonNextMessage.Enabled = false;
			}
			else {
				int	selectedIndex = listViewMessages.SelectedIndices[0];

				buttonPrevMessage.Enabled = selectedIndex > 0;
				buttonNextMessage.Enabled = selectedIndex < listViewMessages.Items.Count-1;
			}

			if(currentMessageSelection != null && currentMessageSelection.Count > 1) {
				buttonNextComponent.Visible = true;
				buttonPreviousComponent.Visible = true;
			}
			else {
				buttonNextComponent.Visible = false;
				buttonPreviousComponent.Visible = false;
			}
		}

		protected void HideSelections() {
			if(currentMessageSelection != null) {
				currentMessageSelection.Hide();
				currentMessageSelection = null;
			}

			if(currentComponentSelection != null) {
				currentComponentSelection.Hide();
				currentComponentSelection = null;
			}
		}

		protected void UpdateSelections() {
			HideSelections();

			MessageItem	item = null;
			if(listViewMessages.SelectedItems.Count > 0)
				item = listViewMessages.SelectedItems[0] as MessageItem;

			if(item != null && item.Selection != null) {
				currentMessageSelection = item.Selection;

				currentMessageSelection.Display(new LayoutSelectionLook(Color.LightPink));

				if(currentMessageSelection.Count > 0) {
					currentComponentSelection = new LayoutSelection();
					setComponentSelection(currentMessageSelection.Components.First());
					currentComponentSelection.Display(new LayoutSelectionLook(Color.DarkRed));
				}
			}
		}

		void setComponentSelection(ModelComponent component) {
			currentComponentSelection.Clear();
			currentComponentSelection.Add(component);
			EventManager.Event(new LayoutEvent(component, "ensure-component-visible", null, false));
		}

		[LayoutEvent("messages-hidden")]
		void MessagesHidden(LayoutEvent e) {
			clearOnNewMessage = true;
			HideSelections();
		}

		[LayoutEvent("messages-shown")]
		void MessagesShown(LayoutEvent e) {
			UpdateSelections();
		}

		[LayoutEvent("clear-messages")]
		void ClearMessages(LayoutEvent e) {
			listViewMessages.Items.Clear();
			listViewMessages.Items.Add(lastMessageMarker);

			UpdateSelections();
			UpdateButtons();
		}

		#region Class that represent a message item

		enum MessageSeverity {
			Message,
			Warning,
			Error
		};

		class MessageItem : ListViewItem {
			MessageSeverity		severity;
			LayoutSelection		selection;

			public MessageItem(MessageSeverity severity, Object messageSubject, String message) {
				String	areaNames = "";

				this.Text = message;
				this.severity = severity;

				if(messageSubject != null) {
					selectMessageSubject(messageSubject);

					// Create areas string
					Hashtable	areas = new Hashtable();

					foreach(ModelComponent component in selection)
						if(!areas.Contains(component.Spot.Area))
							areas.Add(component.Spot.Area, component);

					foreach(LayoutModelArea area in areas.Keys) {
						if(areaNames.Length > 0)
							areaNames += ", ";
						areaNames += area.Name;
					}

					if(selection.Count > 1)
						areaNames = selection.Count + " components in " + areaNames;
				}

				this.SubItems.Add(areaNames);

				switch(severity) {
					case MessageSeverity.Message:
						this.ImageIndex = 0;
						break;

					case MessageSeverity.Warning:
						this.ImageIndex = 1;
						break;

					case MessageSeverity.Error:
						this.ImageIndex = 2;
						break;
				}
			}

			private void selectMessageSubject(object messageSubject) {
				if(messageSubject is LayoutSelection)
					this.selection = (LayoutSelection)messageSubject;
				else if(messageSubject is ModelComponent) {
					ModelComponent		component = (ModelComponent)messageSubject;

					selection = new LayoutSelection(new ModelComponent[] { component });
				}
				else if(messageSubject is TrainStateInfo) {
					TrainStateInfo	train = (TrainStateInfo)messageSubject;
					
					selection = new LayoutSelection();
					selection.Add(train);
				}
				else if(messageSubject is LayoutBlock) {
					LayoutBlock	block = (LayoutBlock)messageSubject;

					if(block.BlockDefinintion != null)
						selection = new LayoutSelection(new ModelComponent[] { block.BlockDefinintion });
					else {
						selection = new LayoutSelection();

						foreach(TrackEdge edge in block.TrackEdges)
							selection.Add(edge.Track);
					}
				}
				else if(messageSubject is LayoutOccupancyBlock) {
					selection = new LayoutSelection();

					foreach(TrackEdge edge in ((LayoutOccupancyBlock)messageSubject).TrackEdges)
						selection.Add(edge.Track);
				}
				else if(messageSubject is TrainStateInfo[]) {
					TrainStateInfo[]			trains = (TrainStateInfo[] )messageSubject;
					ArrayList					blockInfos = new ArrayList();

					selection = new LayoutSelection();

					foreach(TrainStateInfo train in trains)
						selection.Add(train);
				}
				else if(messageSubject is Guid) {
					Guid			id = (Guid)messageSubject;
					var				component = LayoutModel.Component<ModelComponent>(id, LayoutPhase.All);

					if(component != null)
						selectMessageSubject(component);
					else {
						LayoutBlock	block = LayoutModel.Blocks[id];

						if(block != null)
							selectMessageSubject(block);
						else {
							TrainStateInfo	train = LayoutModel.StateManager.Trains[id];

							if(train != null)
								selectMessageSubject(train);
							else
								throw new ArgumentException("The ID " + id.ToString() + " given as message subject can not be associated with an object");
						}
					}
				}
				else
					throw new ArgumentException("Invalid messageSubject - can be either selection or component", messageSubject.ToString());
			}

            public LayoutSelection Selection => selection;

            public MessageSeverity Severity => severity;

            public void TraceMessage() {
				bool	show = false;

				if(Severity == MessageSeverity.Error && traceMessages.TraceError)
					show = true;
				else if(Severity == MessageSeverity.Warning && traceMessages.TraceWarning)
					show = true;
				else if(Severity == MessageSeverity.Message && traceMessages.TraceInfo)
					show = true;

				if(show) {
					bool	first = true;

					Trace.WriteLine("*** " + Severity.ToString() + " -- " + Text);
					Trace.Write("    at: ");

					if(Selection != null) {
						foreach(ModelComponent component in Selection)
							Trace.Write((first ? "" : ", ") + component.FullDescription);
					}
					Trace.WriteLine("");
				}
			}
		}

		#endregion

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MessageViewer));
			this.listViewMessages = new ListView();
			this.columnHeaderMessage = new ColumnHeader();
			this.columnHeaderArea = new ColumnHeader();
			this.buttonClose = new Button();
			this.buttonNextMessage = new Button();
			this.buttonPrevMessage = new Button();
			this.buttonNextComponent = new Button();
			this.buttonPreviousComponent = new Button();
			this.imageListSeverity = new ImageList(this.components);
			this.SuspendLayout();
			// 
			// listViewMessages
			// 
			this.listViewMessages.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewMessages.Columns.AddRange(new ColumnHeader[] {
																							   this.columnHeaderMessage,
																							   this.columnHeaderArea});
			this.listViewMessages.FullRowSelect = true;
			this.listViewMessages.GridLines = true;
			this.listViewMessages.HideSelection = false;
			this.listViewMessages.Location = new System.Drawing.Point(0, 24);
			this.listViewMessages.MultiSelect = false;
			this.listViewMessages.Name = "listViewMessages";
			this.listViewMessages.Size = new System.Drawing.Size(680, 80);
			this.listViewMessages.SmallImageList = this.imageListSeverity;
			this.listViewMessages.TabIndex = 5;
			this.listViewMessages.View = System.Windows.Forms.View.Details;
			this.listViewMessages.SelectedIndexChanged += new System.EventHandler(this.listViewMessages_SelectedIndexChanged);
			// 
			// columnHeaderMessage
			// 
			this.columnHeaderMessage.Text = "Message";
			this.columnHeaderMessage.Width = 393;
			// 
			// columnHeaderArea
			// 
			this.columnHeaderArea.Text = "Area";
			this.columnHeaderArea.Width = 180;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(598, 3);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 20);
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "&Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// buttonNextMessage
			// 
			this.buttonNextMessage.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonNextMessage.Location = new System.Drawing.Point(352, 3);
			this.buttonNextMessage.Name = "buttonNextMessage";
			this.buttonNextMessage.Size = new System.Drawing.Size(112, 20);
			this.buttonNextMessage.TabIndex = 2;
			this.buttonNextMessage.Text = "&Next message";
			this.buttonNextMessage.Click += new System.EventHandler(this.buttonNextMessage_Click);
			// 
			// buttonPrevMessage
			// 
			this.buttonPrevMessage.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonPrevMessage.Location = new System.Drawing.Point(472, 3);
			this.buttonPrevMessage.Name = "buttonPrevMessage";
			this.buttonPrevMessage.Size = new System.Drawing.Size(112, 20);
			this.buttonPrevMessage.TabIndex = 3;
			this.buttonPrevMessage.Text = "&Previous message";
			this.buttonPrevMessage.Click += new System.EventHandler(this.buttonPrevMessage_Click);
			// 
			// buttonNextComponent
			// 
			this.buttonNextComponent.Location = new System.Drawing.Point(8, 3);
			this.buttonNextComponent.Name = "buttonNextComponent";
			this.buttonNextComponent.Size = new System.Drawing.Size(120, 20);
			this.buttonNextComponent.TabIndex = 0;
			this.buttonNextComponent.Text = "Next component";
			this.buttonNextComponent.Click += new System.EventHandler(this.buttonNextComponent_Click);
			// 
			// buttonPreviousComponent
			// 
			this.buttonPreviousComponent.Location = new System.Drawing.Point(135, 3);
			this.buttonPreviousComponent.Name = "buttonPreviousComponent";
			this.buttonPreviousComponent.Size = new System.Drawing.Size(120, 20);
			this.buttonPreviousComponent.TabIndex = 1;
			this.buttonPreviousComponent.Text = "Previous component";
			this.buttonPreviousComponent.Click += new System.EventHandler(this.buttonPreviousComponent_Click);
			// 
			// imageListSeverity
			// 
			this.imageListSeverity.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListSeverity.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListSeverity.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSeverity.ImageStream")));
			this.imageListSeverity.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MessageViewer
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonPreviousComponent,
																		  this.buttonNextComponent,
																		  this.buttonPrevMessage,
																		  this.buttonNextMessage,
																		  this.buttonClose,
																		  this.listViewMessages});
			this.Name = "MessageViewer";
			this.Size = new System.Drawing.Size(680, 112);
			this.ResumeLayout(false);

		}
		#endregion


		private void buttonClose_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "hide-messages"));
		}

		private void listViewMessages_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateSelections();
			UpdateButtons();
		}

		private void buttonNextComponent_Click(object sender, System.EventArgs e) {
			if(currentComponentSelection != null) {
				IList<ModelComponent>	messageComponents = currentMessageSelection.Components.ToList();
				ModelComponent			currentComponent = currentComponentSelection.Components.FirstOrDefault() ?? messageComponents[0];
				int						currentComponentIndex = -1;

				for(int i = 0; i < messageComponents.Count; i++)
					if(messageComponents[i] == currentComponent) {
						currentComponentIndex = i;
						break;
					}

				if(currentComponentIndex >= 0) {
					if(++currentComponentIndex == messageComponents.Count)
						currentComponentIndex = 0;

					setComponentSelection(messageComponents[currentComponentIndex]);
				}
			}
		}

		private void buttonPreviousComponent_Click(object sender, System.EventArgs e) {
			if(currentComponentSelection != null) {
				IList<ModelComponent> messageComponents = currentMessageSelection.Components.ToList();
				ModelComponent currentComponent = currentComponentSelection.Components.FirstOrDefault() ?? messageComponents[0];
				int currentComponentIndex = -1;

				for(int i = 0; i < messageComponents.Count; i++)
					if(messageComponents[i] == currentComponent) {
						currentComponentIndex = i;
						break;
					}

				if(currentComponentIndex >= 0) {
					if(currentComponentIndex-- == 0)
						currentComponentIndex = messageComponents.Count-1;

					setComponentSelection(messageComponents[currentComponentIndex]);
				}
			}
		}

		private void buttonNextMessage_Click(object sender, System.EventArgs e) {
			if(listViewMessages.SelectedItems.Count > 0) {
				int		i;

				i = listViewMessages.SelectedIndices[0];

				if(++i < listViewMessages.Items.Count) {
					listViewMessages.SelectedItems[0].Selected = false;
					listViewMessages.Items[i].Selected = true;
				}
			}
		}

		private void buttonPrevMessage_Click(object sender, System.EventArgs e) {
			if(listViewMessages.SelectedItems.Count > 0) {
				int		i;

				i = listViewMessages.SelectedIndices[0];

				if(i > 0) {
					i--;
					listViewMessages.SelectedItems[0].Selected = false;
					listViewMessages.Items[i].Selected = true;
				}
			}
		}
	}
}
