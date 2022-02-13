using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.Tools.Controls {
    /// <summary>
    /// Summary description for TripsMonitor.
    /// </summary>
    public partial class TripsMonitor : System.Windows.Forms.UserControl {
        private const string A_TripsMonitorAutoClearTimeout = "TripsMonitorAutoClearTimeout";

        private readonly IDictionary mapTrainToItem = new HybridDictionary();
        private int autoClearTimeout;      // Clear completed trips after this amount of time
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private CommonUI.ListViewStringColumnsSorter? columnSorter;
        private ILayoutFrameWindow? frameWindow = null;

        public TripsMonitor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public void Initialize() {
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            autoClearTimeout = (int?)LayoutModel.StateManager.Element.AttributeValue(A_TripsMonitorAutoClearTimeout) ?? 15;
            columnSorter = new CommonUI.ListViewStringColumnsSorter(listViewTrips);
        }

        public int AutoClearTimeout => autoClearTimeout;

        public ILayoutFrameWindow FrameWindow => frameWindow ??= (ILayoutFrameWindow)FindForm();

        public void AddTripAssignment(TripPlanAssignmentInfo tripAssignment) {
            var item = (TripAssignmentItem?)mapTrainToItem[tripAssignment.TrainId];

            if (item == null) {
                item = new TripAssignmentItem(this, tripAssignment);
                listViewTrips.Items.Add(item);
                mapTrainToItem.Add(tripAssignment.TrainId, item);
            }
            else
                item.TripAssignment = tripAssignment;

            listViewTrips.Sort();
        }

        private void RemoveTripAssignment(TripAssignmentItem item) {
            item.Dispose();
            mapTrainToItem.Remove(item.Train.Id);
            listViewTrips.Items.Remove(item);
        }

        private void RemoveTripAssignment(Guid trainID) {
            var item = GetTripAssignmentItem(trainID);

            if (item != null)
                RemoveTripAssignment(item);
        }

        private TripAssignmentItem? GetTripAssignmentItem(Guid trainID) => (TripAssignmentItem?)mapTrainToItem[trainID];

        private TripAssignmentItem? GetTripAssignmentItem(TrainStateInfo train) => GetTripAssignmentItem(train.Id);

        private TripAssignmentItem? GetTripAssignmentItem(TripPlanAssignmentInfo tripPlanAssignment) => GetTripAssignmentItem(tripPlanAssignment.TrainId);

        private TripAssignmentItem? GetSelected() {
            TripAssignmentItem? selected = null;

            if (listViewTrips.SelectedItems.Count > 0)
                selected = (TripAssignmentItem)listViewTrips.SelectedItems[0];

            return selected;
        }

        private void UpdateAutoClearTimers() {
            foreach (TripAssignmentItem item in listViewTrips.Items)
                item.UpdateAutoClearTimer();
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            var selected = GetSelected();

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
        private void TripAdded(LayoutEvent e) {
            var tripAssignment = Ensure.NotNull<TripPlanAssignmentInfo>(e.Sender);

            AddTripAssignment(tripAssignment);
        }

        [LayoutEvent("trip-cleared")]
        private void TripCleared(LayoutEvent e) {
            var tripAssignment = Ensure.NotNull<TripPlanAssignmentInfo>(e.Sender);

            RemoveTripAssignment(tripAssignment.TrainId);
        }

        [LayoutEvent("trip-status-changed")]
        private void TripStatusChanged(LayoutEvent e) {
            var trip = Ensure.NotNull<TripPlanAssignmentInfo>(e.Sender);
            var item = GetTripAssignmentItem(trip);

            if (item != null)
                item.UpdateTripState();
            UpdateButtons(null, EventArgs.Empty);
        }

        [DispatchTarget]
        private void OnTrainSpeedChanged(TrainStateInfo train, int speed) {
            var item = GetTripAssignmentItem(train.Id);

            if (item != null)
                item.UpdateTrainStatus();
        }

        [LayoutEvent("train-enter-block")]
        private void TrainStatusChanged(LayoutEvent e) {
            var train = Ensure.NotNull<TrainsStateInfo>(e.Sender);
            var item = GetTripAssignmentItem(train.Id);

            if (item != null)
                item.UpdateTrainStatus();
        }

        [LayoutEvent("prepare-enter-operation-mode")]
        private void EnterOperationMode(LayoutEvent e) {
            listViewTrips.Items.Clear();
            mapTrainToItem.Clear();
            UpdateButtons(null, EventArgs.Empty);
        }

        [LayoutEvent("train-is-removed")]
        private void TrainRemoved(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender);

            RemoveTripAssignment(train.Id);
        }

        #endregion

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("hide-trips-monitor", this).SetFrameWindow((ILayoutFrameWindow)FindForm()));
        }

        private void ButtonSave_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null)
                EventManager.Event(new LayoutEvent("save-trip-plan", selected.TripAssignment.TripPlan, this.ParentForm));
        }

        private void ButtonView_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();


            if (selected != null) {
                selected.CancelAutoClearTimer();

                Dialogs.TripPlanViewing tripPlanView = new(selected.Train, selected.TripAssignment.TripPlan, selected.TripAssignment.CurrentWaypointIndex);
                tripPlanView.FormClosed += (_, _) => selected.UpdateAutoClearTimer();
                tripPlanView.Show(FindForm());
            }
        }

        private void ListViewTrips_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null) {
                if (selected.Train.LocomotiveBlock != null) {
                    ModelComponent component = selected.Train.LocomotiveBlock.BlockDefinintion;

                    if (component != null)
                        EventManager.Event(new LayoutEvent("ensure-component-visible", component, true).SetFrameWindow(FrameWindow));
                }
            }

            UpdateButtons(null, EventArgs.Empty);
        }

        private void ButtonOptions_Click(object? sender, System.EventArgs e) {
            using Dialogs.TripsMonitorOptions d = new(autoClearTimeout);

            if (d.ShowDialog(this.ParentForm) == DialogResult.OK) {
                autoClearTimeout = d.AutoClearTimeout;

                if (autoClearTimeout < 0) {
                    UpdateAutoClearTimers();
                    LayoutModel.StateManager.Element.RemoveAttribute(A_TripsMonitorAutoClearTimeout);
                }
                else {
                    UpdateAutoClearTimers();
                    LayoutModel.StateManager.Element.SetAttributeValue(A_TripsMonitorAutoClearTimeout, autoClearTimeout);
                }
            }
        }

        private void ButtonAbort_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null) {
                if (selected.TripAssignment.CanBeCleared)
                    EventManager.Event(new LayoutEvent("clear-trip", selected.Train));
                else
                    EventManager.Event(new LayoutEvent("abort-trip", selected.Train, true));
            }
        }

        private void ButtonSuspend_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null) {
                if (selected.TripAssignment.Status == TripStatus.Suspended)
                    EventManager.Event(new LayoutEvent("resume-trip", selected.Train));
                else
                    EventManager.Event(new LayoutEvent("suspend-trip", selected.Train, true));
            }
        }

        private void ButtonSpeed_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null && selected.Train.Driver.Type == "Automatic")
                EventManager.Event(new LayoutEvent("get-train-target-speed", selected.TripAssignment.Train, this));
        }

        private class TripAssignmentItem : ListViewItem, IDisposable {
            private readonly TripsMonitor tripsMonitor;
            private TripPlanAssignmentInfo tripAssignment;
            private LayoutDelayedEvent? clearTripEvent = null;

            public TripAssignmentItem(TripsMonitor tripsMonitor, TripPlanAssignmentInfo tripAssignment) {
                this.tripsMonitor = tripsMonitor;
                this.tripAssignment = tripAssignment;
                this.Train = tripAssignment.Train;

                Text = Train.DisplayName;
                ImageIndex = GetStateImageIndex();
                SubItems.Add(GetDriverName());
                SubItems.Add(Train.StatusText);
                SubItems.Add(GetDestination());
                SubItems.Add(GetTripState());
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
                SubItems[1].Text = GetDriverName();
            }

            public void UpdateTrainStatus() {
                SubItems[2].Text = Train.StatusText;
            }

            public void UpdateDestination() {
                SubItems[3].Text = GetDestination();
            }

            public void UpdateTripState() {
                ImageIndex = GetStateImageIndex();
                SubItems[4].Text = GetTripState();

                UpdateAutoClearTimer();
            }

            #endregion

            #region Format field strings

            private string GetDriverName() => tripAssignment.Train.Driver.Type switch
            {
                "ManualOnScreen" => "Manual (Dialog)",
                _ => tripAssignment.Train.Driver.Type.ToString(),
            };

            private string GetDestination() {
                IList<TripPlanWaypointInfo> wayPoints = tripAssignment.TripPlan.Waypoints;

                return wayPoints.Count < 1 ? "<Empty>" : wayPoints[wayPoints.Count - 1].Destination.Name;
            }

            private string GetTripState() => tripAssignment.Status switch
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

            private int GetStateImageIndex() => tripAssignment.Status switch
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
