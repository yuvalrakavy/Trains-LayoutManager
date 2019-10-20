using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060, IDE0052, IDE0069
namespace LayoutManager.Tools.Controls {
    /// <summary>
    /// Summary description for TripsMonitor.
    /// </summary>
    public class TripsMonitor : System.Windows.Forms.UserControl {
        private const string A_TripsMonitorAutoClearTimeout = "TripsMonitorAutoClearTimeout";
        private ListView listViewTrips;
        private Button buttonAbort;
        private ColumnHeader columnHeaderTrain;
        private ColumnHeader columnHeaderStatus;
        private ColumnHeader columnHeaderDriver;
        private ColumnHeader columnHeaderFinalDestination;
        private ColumnHeader columnHeaderState;
        private Button buttonClose;
        private Button buttonView;
        private Button buttonSave;
        private Button buttonTalk;
        private Button buttonOptions;
        private ImageList imageListTripState;
        private Button buttonSpeed;
        private Button buttonSuspend;
        private IContainer components;

        private void endOfDesignerVariables() { }

        private readonly IDictionary mapTrainToItem = new HybridDictionary();
        private int autoClearTimeout;      // Clear completed trips after this amount of time
        private CommonUI.ListViewStringColumnsSorter columnSorter;
        private ILayoutFrameWindow frameWindow = null;

        public TripsMonitor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public void Initialize() {
            EventManager.AddObjectSubscriptions(this);

            autoClearTimeout = (int?)LayoutModel.StateManager.Element.AttributeValue(A_TripsMonitorAutoClearTimeout) ?? 15;
            columnSorter = new CommonUI.ListViewStringColumnsSorter(listViewTrips);
        }

        public int AutoClearTimeout => autoClearTimeout;

        public ILayoutFrameWindow FrameWindow => frameWindow ?? (frameWindow = (ILayoutFrameWindow)FindForm());

        public void AddTripAssignment(TripPlanAssignmentInfo tripAssignment) {
            TripAssignmentItem item = (TripAssignmentItem)mapTrainToItem[tripAssignment.TrainId];

            if (item == null) {
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
            TripAssignmentItem item = getTripAssignmentItem(trainID);

            if (item != null)
                removeTripAssignment(item);
        }

        private TripAssignmentItem getTripAssignmentItem(Guid trainID) => (TripAssignmentItem)mapTrainToItem[trainID];

        private TripAssignmentItem getTripAssignmentItem(TrainStateInfo train) => getTripAssignmentItem(train.Id);

        private TripAssignmentItem getTripAssignmentItem(TripPlanAssignmentInfo tripPlanAssignment) => getTripAssignmentItem(tripPlanAssignment.TrainId);

        private TripAssignmentItem getSelected() {
            TripAssignmentItem selected = null;

            if (listViewTrips.SelectedItems.Count > 0)
                selected = (TripAssignmentItem)listViewTrips.SelectedItems[0];

            return selected;
        }

        private void updateAutoClearTimers() {
            foreach (TripAssignmentItem item in listViewTrips.Items)
                item.UpdateAutoClearTimer();
        }

        private void updateButtons(object sender, EventArgs e) {
            TripAssignmentItem selected = getSelected();

            if (selected != null) {
                buttonAbort.Enabled = true;
                buttonSuspend.Enabled = true;
                buttonView.Enabled = true;
                buttonSave.Enabled = true;
                buttonTalk.Enabled = false;     // TODO: Set based on the train driver capabilities

                if (selected.Train.Driver.Type == "Automatic")
                    buttonSpeed.Enabled = true;
                else
                    buttonSpeed.Enabled = false;

                if (selected.TripAssignment.CanBeCleared) {
                    buttonSuspend.Enabled = false;
                    buttonAbort.Text = "&Clear";
                }
                else
                    buttonAbort.Text = "&Abort";

                if (selected.TripAssignment.Status == TripStatus.Suspended)
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
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    EventManager.Subscriptions.RemoveObjectSubscriptions(this);
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Layout Event Handlers

        [LayoutEvent("trip-added")]
        private void tripAdded(LayoutEvent e) {
            TripPlanAssignmentInfo tripAssignment = (TripPlanAssignmentInfo)e.Sender;

            AddTripAssignment(tripAssignment);
        }

        [LayoutEvent("trip-cleared")]
        private void tripCleared(LayoutEvent e) {
            TripPlanAssignmentInfo tripAssignment = (TripPlanAssignmentInfo)e.Sender;

            removeTripAssignment(tripAssignment.TrainId);
        }

        [LayoutEvent("trip-status-changed")]
        private void tripStatusChanged(LayoutEvent e) {
            TripAssignmentItem item = getTripAssignmentItem((TripPlanAssignmentInfo)e.Sender);

            if (item != null)
                item.UpdateTripState();
            updateButtons(null, null);
        }

        [LayoutEvent("train-speed-changed")]
        [LayoutEvent("train-enter-block")]
        private void trainStatusChanged(LayoutEvent e) {
            TripAssignmentItem item = getTripAssignmentItem((TrainStateInfo)e.Sender);

            if (item != null)
                item.UpdateTrainStatus();
        }

        [LayoutEvent("prepare-enter-operation-mode")]
        private void enterOperationMode(LayoutEvent e) {
            listViewTrips.Items.Clear();
            mapTrainToItem.Clear();
            updateButtons(null, null);
        }

        [LayoutEvent("train-is-removed")]
        private void trainRemoved(LayoutEvent e) {
            TrainStateInfo train = (TrainStateInfo)e.Sender;

            removeTripAssignment(train.Id);
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TripsMonitor));
            this.listViewTrips = new ListView();
            this.columnHeaderTrain = new ColumnHeader();
            this.columnHeaderDriver = new ColumnHeader();
            this.columnHeaderStatus = new ColumnHeader();
            this.columnHeaderFinalDestination = new ColumnHeader();
            this.columnHeaderState = new ColumnHeader();
            this.imageListTripState = new ImageList(this.components);
            this.buttonAbort = new Button();
            this.buttonClose = new Button();
            this.buttonView = new Button();
            this.buttonSave = new Button();
            this.buttonTalk = new Button();
            this.buttonOptions = new Button();
            this.buttonSpeed = new Button();
            this.buttonSuspend = new Button();
            this.SuspendLayout();
            // 
            // listViewTrips
            // 
            this.listViewTrips.AllowColumnReorder = true;
            this.listViewTrips.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewTrips.Columns.AddRange(new ColumnHeader[] {
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
            this.listViewTrips.Click += this.listViewTrips_Click;
            this.listViewTrips.SelectedIndexChanged += this.updateButtons;
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
            this.imageListTripState.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListTripState.ImageStream");
            this.imageListTripState.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonAbort
            // 
            this.buttonAbort.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAbort.Location = new System.Drawing.Point(8, 136);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 19);
            this.buttonAbort.TabIndex = 1;
            this.buttonAbort.Text = "&Abort";
            this.buttonAbort.Click += this.buttonAbort_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(589, 136);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 19);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // buttonView
            // 
            this.buttonView.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonView.Location = new System.Drawing.Point(168, 136);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(75, 19);
            this.buttonView.TabIndex = 3;
            this.buttonView.Text = "&View...";
            this.buttonView.Click += this.buttonView_Click;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonSave.Location = new System.Drawing.Point(248, 136);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 19);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "&Save...";
            this.buttonSave.Click += this.buttonSave_Click;
            // 
            // buttonTalk
            // 
            this.buttonTalk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonTalk.Location = new System.Drawing.Point(328, 136);
            this.buttonTalk.Name = "buttonTalk";
            this.buttonTalk.Size = new System.Drawing.Size(75, 19);
            this.buttonTalk.TabIndex = 5;
            this.buttonTalk.Text = "&Talk";
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonOptions.Location = new System.Drawing.Point(488, 136);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(75, 19);
            this.buttonOptions.TabIndex = 7;
            this.buttonOptions.Text = "&Options...";
            this.buttonOptions.Click += this.buttonOptions_Click;
            // 
            // buttonSpeed
            // 
            this.buttonSpeed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonSpeed.Location = new System.Drawing.Point(408, 136);
            this.buttonSpeed.Name = "buttonSpeed";
            this.buttonSpeed.Size = new System.Drawing.Size(75, 19);
            this.buttonSpeed.TabIndex = 6;
            this.buttonSpeed.Text = "S&peed...";
            this.buttonSpeed.Click += this.buttonSpeed_Click;
            // 
            // buttonSuspend
            // 
            this.buttonSuspend.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonSuspend.Location = new System.Drawing.Point(88, 136);
            this.buttonSuspend.Name = "buttonSuspend";
            this.buttonSuspend.Size = new System.Drawing.Size(75, 19);
            this.buttonSuspend.TabIndex = 2;
            this.buttonSuspend.Text = "&Suspend";
            this.buttonSuspend.Click += this.buttonSuspend_Click;
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
            EventManager.Event(new LayoutEvent("hide-trips-monitor", this).SetFrameWindow((ILayoutFrameWindow)FindForm()));
        }

        private void buttonSave_Click(object sender, System.EventArgs e) {
            TripAssignmentItem selected = getSelected();

            if (selected != null)
                EventManager.Event(new LayoutEvent("save-trip-plan", selected.TripAssignment.TripPlan, this.ParentForm));
        }

        private void buttonView_Click(object sender, System.EventArgs e) {
            TripAssignmentItem selected = getSelected();

            selected.CancelAutoClearTimer();

            if (selected != null) {
#pragma warning disable IDE0067 // Dispose objects before losing scope
                Dialogs.TripPlanViewing tripPlanView = new Dialogs.TripPlanViewing(selected.Train, selected.TripAssignment.TripPlan, selected.TripAssignment.CurrentWaypointIndex);
#pragma warning restore IDE0067 // Dispose objects before losing scope

                tripPlanView.FormClosed += (object s1, FormClosedEventArgs e1) => selected.UpdateAutoClearTimer();
                tripPlanView.Show(FindForm());
            }
        }

        private void listViewTrips_Click(object sender, System.EventArgs e) {
            TripAssignmentItem selected = getSelected();

            if (selected != null) {
                if (selected.Train.LocomotiveBlock != null) {
                    ModelComponent component = selected.Train.LocomotiveBlock.BlockDefinintion;

                    if (component != null)
                        EventManager.Event(new LayoutEvent("ensure-component-visible", component, true).SetFrameWindow(FrameWindow));
                }
            }

            updateButtons(null, null);
        }

        private void buttonOptions_Click(object sender, System.EventArgs e) {
            using Dialogs.TripsMonitorOptions d = new Dialogs.TripsMonitorOptions(autoClearTimeout);

            if (d.ShowDialog(this.ParentForm) == DialogResult.OK) {
                autoClearTimeout = d.AutoClearTimeout;

                if (autoClearTimeout < 0) {
                    updateAutoClearTimers();
                    LayoutModel.StateManager.Element.RemoveAttribute(A_TripsMonitorAutoClearTimeout);
                }
                else {
                    updateAutoClearTimers();
                    LayoutModel.StateManager.Element.SetAttributeValue(A_TripsMonitorAutoClearTimeout, autoClearTimeout);
                }
            }
        }

        private void buttonAbort_Click(object sender, System.EventArgs e) {
            TripAssignmentItem selected = getSelected();

            if (selected != null) {
                if (selected.TripAssignment.CanBeCleared)
                    EventManager.Event(new LayoutEvent("clear-trip", selected.Train));
                else
                    EventManager.Event(new LayoutEvent("abort-trip", selected.Train, true));
            }
        }

        private void buttonSuspend_Click(object sender, System.EventArgs e) {
            TripAssignmentItem selected = getSelected();

            if (selected != null) {
                if (selected.TripAssignment.Status == TripStatus.Suspended)
                    EventManager.Event(new LayoutEvent("resume-trip", selected.Train));
                else
                    EventManager.Event(new LayoutEvent("suspend-trip", selected.Train, true));
            }
        }

        private void buttonSpeed_Click(object sender, System.EventArgs e) {
            TripAssignmentItem selected = getSelected();

            if (selected != null && selected.Train.Driver.Type == "Automatic")
                EventManager.Event(new LayoutEvent("get-train-target-speed", selected.TripAssignment.Train, this));
        }

        private class TripAssignmentItem : ListViewItem, IDisposable {
            private readonly TripsMonitor tripsMonitor;
            private TripPlanAssignmentInfo tripAssignment;
            private LayoutDelayedEvent clearTripEvent = null;

            public TripAssignmentItem(TripsMonitor tripsMonitor, TripPlanAssignmentInfo tripAssignment) {
                this.tripsMonitor = tripsMonitor;
                this.tripAssignment = tripAssignment;
                this.Train = tripAssignment.Train;

                Text = Train.DisplayName;
                ImageIndex = getStateImageIndex();
                SubItems.Add(getDriverName());
                SubItems.Add(Train.StatusText);
                SubItems.Add(getDestination());
                SubItems.Add(getTripState());
            }

            #region Properties

            public TrainStateInfo Train { get; }

            public TripPlanAssignmentInfo TripAssignment {
                get {
                    return tripAssignment;
                }

                set {
                    if (value.TrainId != Train.Id)
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
                SubItems[2].Text = Train.StatusText;
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

            private string getDriverName() => tripAssignment.Train.Driver.Type switch
            {
                "ManualOnScreen" => "Manual (Dialog)",
                _ => tripAssignment.Train.Driver.Type.ToString(),
            };

            private string getDestination() {
                IList<TripPlanWaypointInfo> wayPoints = tripAssignment.TripPlan.Waypoints;

                return wayPoints.Count < 1 ? "<Empty>" : wayPoints[wayPoints.Count - 1].Destination.Name;
            }

            private string getTripState() => tripAssignment.Status switch
            {
                TripStatus.NotSubmitted => "New - not submitted",
                TripStatus.Go => "Allowed to move",
                TripStatus.PrepareStop => "Prepare to stop",
                TripStatus.Stopped => "Must stop!",
                TripStatus.WaitLock => "Wait for green light",
                TripStatus.WaitStartCondition => "Wait to start next segment",
                TripStatus.Suspended => "Trip suspended",
                TripStatus.Aborted => "Trip aborted!",
                TripStatus.Done => "Trip done!",
                _ => tripAssignment.Status.ToString(),
            };

            private int getStateImageIndex() => tripAssignment.Status switch
            {
                TripStatus.NotSubmitted => 0,
                TripStatus.Go => 1,
                TripStatus.PrepareStop => 2,
                TripStatus.Stopped => 3,
                TripStatus.WaitLock => 4,
                TripStatus.WaitStartCondition => 5,
                TripStatus.Suspended => 6,
                TripStatus.Aborted => 7,
                TripStatus.Done => 8,
                _ => 0,
            };

            #endregion

            #region Auto clear timer handling

            public void CancelAutoClearTimer() {
                if (clearTripEvent != null) {
                    clearTripEvent.Cancel();
                    clearTripEvent = null;
                }
            }

            public void UpdateAutoClearTimer() {
                CancelAutoClearTimer();

                if (tripAssignment.CanBeCleared) {
                    if (tripsMonitor.AutoClearTimeout > 0)
                        clearTripEvent = EventManager.DelayedEvent(tripsMonitor.AutoClearTimeout * 1000, new LayoutEvent("clear-trip", Train));
                }
            }

            public void Dispose() {
                CancelAutoClearTimer();
            }

            #endregion
        }
    }
}
