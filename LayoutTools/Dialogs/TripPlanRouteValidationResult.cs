using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanRouteValidationResult.
    /// </summary>
    public partial class TripPlanRouteValidationResult : Form {
        private readonly Dialogs.TripPlanEditor tripPlanEditor;

        public TripPlanRouteValidationResult(TripPlanInfo tripPlan, ITripRouteValidationResult routeValidationResult, Dialogs.TripPlanEditor tripPlanEditor) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.tripPlanEditor = tripPlanEditor;

            if (!routeValidationResult.CanBeFixed) {
                labelDescription.Text = "The train cannot follow the trip plan. Some routes are not possible";
                buttonFixTripPlan.Visible = false;
            }

            foreach (ITripRouteValidationAction action in routeValidationResult.Actions)
                listViewActions.Items.Add(new RouteValidationActionItem(tripPlan, action));
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

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonFixTripPlan_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ListViewActions_SelectedIndexChanged(object? sender, System.EventArgs e) {
            if (listViewActions.SelectedItems.Count > 0) {
                RouteValidationActionItem selected = (RouteValidationActionItem)listViewActions.SelectedItems[0];

                tripPlanEditor.SelectWayPoint(selected.Action.WaypointIndex);
            }
        }

        private class RouteValidationActionItem : ListViewItem {
            public RouteValidationActionItem(TripPlanInfo tripPlan, ITripRouteValidationAction action) {
                this.Action = action;

                this.Text = tripPlan.Waypoints[action.WaypointIndex].Destination.Name;
                this.SubItems.Add(action.Description);
            }

            public ITripRouteValidationAction Action { get; }
        }
    }
}
