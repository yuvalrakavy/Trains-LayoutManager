using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanEditor.
    /// </summary>
    public partial class TripPlanEditor : Form {
        private readonly TrainStateInfo train;

        public TripPlanEditor(TripPlanInfo tripPlan, TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            tripPlanEditor1.Initialize();
            tripPlanEditor1.SetDialogName("Trip plan for " + train.DisplayName);
            tripPlanEditor1.TripPlanName = train.Name;
            tripPlanEditor1.Train = train;
            tripPlanEditor1.TripPlan = tripPlan;

            trainDriverComboBox.Train = train;

            this.Text = tripPlanEditor1.DialogName;

            UpdateButtons(null, EventArgs.Empty);

            EventManager.AddObjectSubscriptions(this);
            this.Owner = LayoutController.ActiveFrameWindow as Form;
        }

        protected void UpdateButtons(object? sender, EventArgs e) {
            if (tripPlanEditor1.Count > 0) {
                buttonGo.Enabled = true;
                buttonSaveTripPlan.Enabled = true;
            }
            else {
                buttonGo.Enabled = false;
                buttonSaveTripPlan.Enabled = false;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        [DispatchTarget]
        private void OnTrainIsRemoved(TrainStateInfo removedTrain) {
            if (removedTrain.Id == train.Id)
                Close();
        }

        [LayoutEvent("query-edit-trip-plan-for-train")]
        private void QueryEditTripPlanForTrain(LayoutEvent e) {
            var queriedTrain = Ensure.NotNull<TrainsStateInfo>(e.Sender);

            if (queriedTrain.Id == train.Id)
                e.Info = tripPlanEditor1.ActiveForm;
        }

        [DispatchTarget]
        private void OnExitOperationMode() {
            Close();
        }

        public void SelectWayPoint(int wayPointIndex) {
            tripPlanEditor1.SelectWayPoint(wayPointIndex);
        }

        private void TripPlanEditor_Closed(object? sender, EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            this.Close();
        }

        private void ButtonGo_Click(object? sender, System.EventArgs e) {
            TripPlanInfo tripPlan = tripPlanEditor1.TripPlan;

            if (!tripPlanEditor1.Check())
                return;

            if (!trainDriverComboBox.ValidateInput())
                return;

            var routeValidationResult = Dispatch.Call.ValidateTripPlanRoute(tripPlan, train);

            if (routeValidationResult.Actions.Count > 0) {
                Dialogs.TripPlanRouteValidationResult d = new(tripPlan, routeValidationResult, this);

                if (d.ShowDialog(this) == DialogResult.OK) {
                    foreach (ITripRouteValidationAction action in routeValidationResult.Actions)
                        action.Apply(tripPlan);
                    tripPlanEditor1.UpdateAll();
                }

                return;
            }

            trainDriverComboBox.Commit();

            try {
                TripPlanAssignmentInfo tripPlanAssignment = new(tripPlan, train);

                if (Dispatch.Call.ExecuteTripPlan(tripPlanAssignment))
                    Close();
            }
            catch (LayoutException ex) {
                ex.Report();
            }
        }

        private void ButtonSaveTripPlan_Click(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("save-trip-plan", tripPlanEditor1.TripPlan));
        }

        private void TripPlanEditor_Closing(object? sender, CancelEventArgs e) {
            tripPlanEditor1.Dispose();
            if (Owner != null)
                Owner.Activate();
        }
    }
}
