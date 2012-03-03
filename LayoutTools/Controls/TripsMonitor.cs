using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Controls
{
	/// <summary>
	/// Summary description for TripsMonitor.
	/// </summary>
	public class TripsMonitor : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ListView listViewTrips;
		private System.Windows.Forms.Button buttonAbort;
		private System.Windows.Forms.ColumnHeader columnHeaderTrain;
		private System.Windows.Forms.ColumnHeader columnHeaderStatus;
		private System.Windows.Forms.ColumnHeader columnHeaderDriver;
		private System.Windows.Forms.ColumnHeader columnHeaderFinalDestination;
		private System.Windows.Forms.ColumnHeader columnHeaderState;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonView;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonTalk;
		private System.Windows.Forms.Button buttonOptions;
		private System.Windows.Forms.ImageList imageListTripState;
		private System.Windows.Forms.Button buttonSpeed;
		private System.Windows.Forms.Button buttonSuspend;
		private System.ComponentModel.IContainer components;

		private void endOfDesignerVariables() { }

		IDictionary								mapTrainToItem = new HybridDictionary();
		int										autoClearTimeout = 15;		// Clear completed trips after this amount of time
		CommonUI.ListViewStringColumnsSorter	columnSorter;
		ILayoutFrameWindow frameWindow = null;

		public TripsMonitor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public void Initialize() {
			EventManager.AddObjectSubscriptions(this);

			if(LayoutModel.StateManager.Element.HasAttribute("TripsMonitorAutoClearTimeout"))
				autoClearTimeout = XmlConvert.ToInt32(LayoutModel.StateManager.Element.GetAttribute("TripsMonitorAutoClearTimeout"));

			columnSorter = new CommonUI.ListViewStringColumnsSorter(listViewTrips);
		}

		public int AutoClearTimeout {
			get {
				return autoClearTimeout;
			}
		}

		public ILayoutFrameWindow FrameWindow {
			get {
				if(frameWindow == null)
					frameWindow = (ILayoutFrameWindow)FindForm();
				return frameWindow;
			}
		}

		public void addTripAssignment(TripPlanAssignmentInfo tripAssignment) {
			TripAssignmentItem	item = (TripAssignmentItem)mapTrainToItem[tripAssignment.TrainId];

			if(item == null) {
				item = new TripAssignmentItem(this, tripAssignment);
				listViewTrips.Items.Add(item);
				mapTrainToItem.Add(tripAssignment.TrainId, item);
			}
			else
				item.TripAssignment = tripAssignment;
			
			listViewTrips.Sort();
		}

		private void removeTripAssignment(TripAssignmentItem item) {
			item.Dispose();
			mapTrainToItem.Remove(item.Train.Id);
			listViewTrips.Items.Remove(item);
		}

		private void removeTripAssignment(Guid trainID) {
			TripAssignmentItem	item = getTripAssignmentItem(trainID);

			if(item != null)
				removeTripAssignment(item);
		}

		private TripAssignmentItem getTripAssignmentItem(Guid trainID) {
			return (TripAssignmentItem)mapTrainToItem[trainID];
		}

		private TripAssignmentItem getTripAssignmentItem(TrainStateInfo train) {
			return getTripAssignmentItem(train.Id);
		}

		private TripAssignmentItem getTripAssignmentItem(TripPlanAssignmentInfo tripPlanAssignment) {
			return getTripAssignmentItem(tripPlanAssignment.TrainId);
		}

		private TripAssignmentItem getSelected() {
			TripAssignmentItem	selected = null;

			if(listViewTrips.SelectedItems.Count > 0)
				selected = (TripAssignmentItem)listViewTrips.SelectedItems[0];

			return selected;
		}

		private void updateAutoClearTimers() {
			foreach(TripAssignmentItem item in listViewTrips.Items)
				item.UpdateAutoClearTimer();
		}

		private void updateButtons(object sender, EventArgs e) {
			TripAssignmentItem	selected = getSelected();

			if(selected != null) {
				buttonAbort.Enabled = true;
				buttonSuspend.Enabled = true;
				buttonView.Enabled = true;
				buttonSave.Enabled = true;
				buttonTalk.Enabled = false;		// TODO: Set based on the train driver capabilities

				if(selected.Train.Driver.Type == "Automatic")
					buttonSpeed.Enabled = true;
				else
					buttonSpeed.Enabled = false;

				if(selected.TripAssignment.CanBeCleared) {
					buttonSuspend.Enabled = false;
					buttonAbort.Text = "&Clear";
				}
				else
					buttonAbort.Text = "&Abort";

				if(selected.TripAssignment.Status == TripStatus.Suspended)
					buttonSuspend.Text = "&Resume";
				else
					buttonSuspend.Text = "&Suspend";
			}
			else {
				buttonAbort.Enabled = false;
				buttonSuspend.Enabled = false;
				buttonView.Enabled = false;
				buttonSave.Enabled = false;
				buttonTalk.Enabled = false;
				buttonSpeed.Enabled = false;
				buttonSuspend.Text = "&Suspend";
				buttonAbort.Text = "&Abort";

			}
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
					EventManager.Subscriptions.RemoveObjectSubscriptions(this);
					components.Dispose();
				}
			}

			base.Dispose( disposing );
		}

		#region Layout Event Handlers

		[LayoutEvent("trip-added")]
		private void tripAdded(LayoutEvent e) {
			TripPlanAssignmentInfo	tripAssignment = (TripPlanAssignmentInfo)e.Sender;

			addTripAssignment(tripAssignment);
		}

		[LayoutEvent("trip-cleared")]
		private void tripCleared(LayoutEvent e) {
			TripPlanAssignmentInfo	tripAssignment = (TripPlanAssignmentInfo)e.Sender;

			removeTripAssignment(tripAssignment.TrainId);
		}

		[LayoutEvent("trip-status-changed")]
		private void tripStatusChanged(LayoutEvent e) {
			TripAssignmentItem	item = getTripAssignmentItem((TripPlanAssignmentInfo)e.Sender);

			if(item != null)
				item.UpdateTripState();
			updateButtons(null, null);
		}

		[LayoutEvent("train-speed-changed")]
		[LayoutEvent("train-enter-block")]
		private void trainStatusChanged(LayoutEvent e) {
			TripAssignmentItem	item = getTripAssignmentItem((TrainStateInfo)e.Sender);

			if(item != null)
				item.UpdateTrainStatus();
		}

		[LayoutEvent("enter-operation-mode")]
		private void enterOperationMode(LayoutEvent e) {
			listViewTrips.Items.Clear();
			mapTrainToItem.Clear();
			updateButtons(null, null);
		}

		[LayoutEvent("train-is-removed")]
		private void trainRemoved(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			removeTripAssignment(train.Id);
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TripsMonitor));
			this.listViewTrips = new System.Windows.Forms.ListView();
			this.columnHeaderTrain = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderDriver = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderStatus = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderFinalDestination = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderState = new System.Windows.Forms.ColumnHeader();
			this.imageListTripState = new System.Windows.Forms.ImageList(this.components);
			this.buttonAbort = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.buttonView = new System.Windows.Forms.Button();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonTalk = new System.Windows.Forms.Button();
			this.buttonOptions = new System.Windows.Forms.Button();
			this.buttonSpeed = new System.Windows.Forms.Button();
			this.buttonSuspend = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listViewTrips
			// 
			this.listViewTrips.AllowColumnReorder = true;
			this.listViewTrips.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewTrips.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeaderTrain,
																							this.columnHeaderDriver,
																							this.columnHeaderStatus,
																							this.columnHeaderFinalDestination,
																							this.columnHeaderState});
			this.listViewTrips.FullRowSelect = true;
			this.listViewTrips.HideSelection = false;
			this.listViewTrips.Location = new System.Drawing.Point(8, 8);
			this.listViewTrips.MultiSelect = false;
			this.listViewTrips.Name = "listViewTrips";
			this.listViewTrips.Size = new System.Drawing.Size(656, 120);
			this.listViewTrips.SmallImageList = this.imageListTripState;
			this.listViewTrips.TabIndex = 0;
			this.listViewTrips.View = System.Windows.Forms.View.Details;
			this.listViewTrips.Click += new System.EventHandler(this.listViewTrips_Click);
			this.listViewTrips.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// columnHeaderTrain
			// 
			this.columnHeaderTrain.Text = "Train";
			this.columnHeaderTrain.Width = 85;
			// 
			// columnHeaderDriver
			// 
			this.columnHeaderDriver.Text = "Driver";
			this.columnHeaderDriver.Width = 107;
			// 
			// columnHeaderStatus
			// 
			this.columnHeaderStatus.Text = "Status";
			this.columnHeaderStatus.Width = 180;
			// 
			// columnHeaderFinalDestination
			// 
			this.columnHeaderFinalDestination.Text = "Destination";
			this.columnHeaderFinalDestination.Width = 160;
			// 
			// columnHeaderState
			// 
			this.columnHeaderState.Text = "State";
			this.columnHeaderState.Width = 120;
			// 
			// imageListTripState
			// 
			this.imageListTripState.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListTripState.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListTripState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTripState.ImageStream")));
			this.imageListTripState.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// buttonAbort
			// 
			this.buttonAbort.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAbort.Location = new System.Drawing.Point(8, 136);
			this.buttonAbort.Name = "buttonAbort";
			this.buttonAbort.Size = new System.Drawing.Size(75, 19);
			this.buttonAbort.TabIndex = 1;
			this.buttonAbort.Text = "&Abort";
			this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(589, 136);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 19);
			this.buttonClose.TabIndex = 8;
			this.buttonClose.Text = "&Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// buttonView
			// 
			this.buttonView.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonView.Location = new System.Drawing.Point(168, 136);
			this.buttonView.Name = "buttonView";
			this.buttonView.Size = new System.Drawing.Size(75, 19);
			this.buttonView.TabIndex = 3;
			this.buttonView.Text = "&View...";
			this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonSave.Location = new System.Drawing.Point(248, 136);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(75, 19);
			this.buttonSave.TabIndex = 4;
			this.buttonSave.Text = "&Save...";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonTalk
			// 
			this.buttonTalk.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonTalk.Location = new System.Drawing.Point(328, 136);
			this.buttonTalk.Name = "buttonTalk";
			this.buttonTalk.Size = new System.Drawing.Size(75, 19);
			this.buttonTalk.TabIndex = 5;
			this.buttonTalk.Text = "&Talk";
			// 
			// buttonOptions
			// 
			this.buttonOptions.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonOptions.Location = new System.Drawing.Point(488, 136);
			this.buttonOptions.Name = "buttonOptions";
			this.buttonOptions.Size = new System.Drawing.Size(75, 19);
			this.buttonOptions.TabIndex = 7;
			this.buttonOptions.Text = "&Options...";
			this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
			// 
			// buttonSpeed
			// 
			this.buttonSpeed.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonSpeed.Location = new System.Drawing.Point(408, 136);
			this.buttonSpeed.Name = "buttonSpeed";
			this.buttonSpeed.Size = new System.Drawing.Size(75, 19);
			this.buttonSpeed.TabIndex = 6;
			this.buttonSpeed.Text = "S&peed...";
			this.buttonSpeed.Click += new System.EventHandler(this.buttonSpeed_Click);
			// 
			// buttonSuspend
			// 
			this.buttonSuspend.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonSuspend.Location = new System.Drawing.Point(88, 136);
			this.buttonSuspend.Name = "buttonSuspend";
			this.buttonSuspend.Size = new System.Drawing.Size(75, 19);
			this.buttonSuspend.TabIndex = 2;
			this.buttonSuspend.Text = "&Suspend";
			this.buttonSuspend.Click += new System.EventHandler(this.buttonSuspend_Click);
			// 
			// TripsMonitor
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonTalk,
																		  this.buttonAbort,
																		  this.listViewTrips,
																		  this.buttonClose,
																		  this.buttonView,
																		  this.buttonSave,
																		  this.buttonOptions,
																		  this.buttonSpeed,
																		  this.buttonSuspend});
			this.Name = "TripsMonitor";
			this.Size = new System.Drawing.Size(672, 160);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonClose_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "hide-trips-monitor").SetFrameWindow((ILayoutFrameWindow)FindForm()));
		}

		private void buttonSave_Click(object sender, System.EventArgs e) {
			TripAssignmentItem		selected = getSelected();

			if(selected != null)
				EventManager.Event(new LayoutEvent(selected.TripAssignment.TripPlan, "save-trip-plan", null, this.ParentForm));
		}

		private void buttonView_Click(object sender, System.EventArgs e) {
			TripAssignmentItem		selected = getSelected();

			selected.CancelAutoClearTimer();

			if(selected != null) {
				Dialogs.TripPlanViewing	tripPlanView = new Dialogs.TripPlanViewing(selected.Train, selected.TripAssignment.TripPlan, selected.TripAssignment.CurrentWaypointIndex);

				tripPlanView.FormClosed += delegate(object s1, FormClosedEventArgs e1) { selected.UpdateAutoClearTimer(); };
				tripPlanView.Show(FindForm());
			}
		}

		private void listViewTrips_Click(object sender, System.EventArgs e) {
			TripAssignmentItem		selected = getSelected();

			if(selected != null) {
                if(selected.Train.LocomotiveBlock != null) {
                    ModelComponent component = selected.Train.LocomotiveBlock.BlockDefinintion;

                    if(component != null)
                        EventManager.Event(new LayoutEvent(component, "ensure-component-visible", null, true).SetFrameWindow(FrameWindow));
                }
			}

			updateButtons(null, null);
		}

		private void buttonOptions_Click(object sender, System.EventArgs e) {
			Dialogs.TripsMonitorOptions	d = new Dialogs.TripsMonitorOptions(autoClearTimeout);

			if(d.ShowDialog(this.ParentForm) == DialogResult.OK) {
				autoClearTimeout = d.AutoClearTimeout;

				if(autoClearTimeout < 0) {
					updateAutoClearTimers();
					LayoutModel.StateManager.Element.RemoveAttribute("TripsMonitorAutoClearTimeout");
				}
				else {
					updateAutoClearTimers();
					LayoutModel.StateManager.Element.SetAttribute("TripsMonitorAutoClearTimeout", XmlConvert.ToString(autoClearTimeout));
				}
			}
		}

		private void buttonAbort_Click(object sender, System.EventArgs e) {
			TripAssignmentItem		selected = getSelected();

			if(selected != null) {
				if(selected.TripAssignment.CanBeCleared)
					EventManager.Event(new LayoutEvent(selected.Train, "clear-trip"));
				else
					EventManager.Event(new LayoutEvent(selected.Train, "abort-trip", null, true));
			}
		}

		private void buttonSuspend_Click(object sender, System.EventArgs e) {
			TripAssignmentItem		selected = getSelected();

			if(selected != null) {
				if(selected.TripAssignment.Status == TripStatus.Suspended)
					EventManager.Event(new LayoutEvent(selected.Train, "resume-trip"));
				else
					EventManager.Event(new LayoutEvent(selected.Train, "suspend-trip", null, true));
			}
		}

		private void buttonSpeed_Click(object sender, System.EventArgs e) {
			TripAssignmentItem		selected = getSelected();

			if(selected != null && selected.Train.Driver.Type == "Automatic")
				EventManager.Event(new LayoutEvent(selected.TripAssignment.Train, "get-train-target-speed", null, this));
		}

		class TripAssignmentItem : ListViewItem, IDisposable {
			TripsMonitor				tripsMonitor;
			TripPlanAssignmentInfo		tripAssignment;
			TrainStateInfo				train;
			LayoutDelayedEvent			clearTripEvent = null;


			public TripAssignmentItem(TripsMonitor tripsMonitor, TripPlanAssignmentInfo tripAssignment) {
				this.tripsMonitor = tripsMonitor;
				this.tripAssignment = tripAssignment;
				this.train = tripAssignment.Train;

				Text = train.DisplayName;
				ImageIndex = getStateImageIndex();
				SubItems.Add(getDriverName());
				SubItems.Add(train.StatusText);
				SubItems.Add(getDestination());
				SubItems.Add(getTripState());
			}

			#region Properties

			public TrainStateInfo Train {
				get {
					return train;
				}
			}

			public TripPlanAssignmentInfo TripAssignment {
				get {
					return tripAssignment;
				}

				set {
					if(value.TrainId != train.Id)
						throw new ArgumentException("Can set trip assignment only to same train");

					tripAssignment = value;
					UpdateDriverName();
					UpdateDestination();
					UpdateTripState();
				}
			}

			#endregion

			#region Operations

			public void UpdateDriverName() {
				SubItems[1].Text = getDriverName();
			}

			public void UpdateTrainStatus() {
				SubItems[2].Text = train.StatusText;
			}

			public void UpdateDestination() {
				SubItems[3].Text = getDestination();
			}

			public void UpdateTripState() {
				ImageIndex = getStateImageIndex();
				SubItems[4].Text = getTripState();

				UpdateAutoClearTimer();
			}

			#endregion

			#region Format field strings

			private string getDriverName() {
				switch(tripAssignment.Train.Driver.Type) {

					case "ManualOnScreen":
						return "Manual (Dialog)";

					default:
						return tripAssignment.Train.Driver.Type.ToString();
				}
			}

			private string getDestination() {
				IList<TripPlanWaypointInfo>	wayPoints = tripAssignment.TripPlan.Waypoints;

				if(wayPoints.Count < 1)
					return "<Empty>";
				return wayPoints[wayPoints.Count - 1].Destination.Name;
			}

			private string getTripState() {
				switch(tripAssignment.Status) {
					case TripStatus.NotSubmitted:
						return "New - not submitted";

					case TripStatus.Go:
						return "Allowed to move";

					case TripStatus.PrepareStop:
						return "Prepare to stop";

					case TripStatus.Stopped:
						return "Must stop!";

					case TripStatus.WaitLock:
						return "Wait for green light";

					case TripStatus.WaitStartCondition:
						return "Wait to start next segment";

					case TripStatus.Suspended:
						return "Trip suspended";

					case TripStatus.Aborted:
						return "Trip aborted!";

					case TripStatus.Done:
						return "Trip done!";

					default:
						return tripAssignment.Status.ToString();
				}
			}

			private int getStateImageIndex() {
				switch(tripAssignment.Status) {
					case TripStatus.NotSubmitted:
						return 0;

					case TripStatus.Go:
						return 1;

					case TripStatus.PrepareStop:
						return 2;

					case TripStatus.Stopped:
						return 3;

					case TripStatus.WaitLock:
						return 4;

					case TripStatus.WaitStartCondition:
						return 5;

					case TripStatus.Suspended:
						return 6;

					case TripStatus.Aborted:
						return 7;

					case TripStatus.Done:
						return 8;

					default:
						return 0;
				}
			}

			#endregion

			#region Auto clear timer handling

			public void CancelAutoClearTimer() {
				if(clearTripEvent != null) {
					clearTripEvent.Cancel();
					clearTripEvent = null;
				}
			}

			public void UpdateAutoClearTimer() {
				CancelAutoClearTimer();

				if(tripAssignment.CanBeCleared) {
					if(tripsMonitor.AutoClearTimeout > 0)
						clearTripEvent = EventManager.DelayedEvent(tripsMonitor.AutoClearTimeout * 1000, new LayoutEvent(train, "clear-trip"));
				}
			}

			public void Dispose() {
				CancelAutoClearTimer();
			}

			#endregion
		}
	}
}
