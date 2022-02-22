using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanViewing.
    /// </summary>
    public partial class TripPlanViewing : Form {
        private readonly TripPlanInfo tripPlan;

        public TripPlanViewing(TrainStateInfo train, TripPlanInfo tripPlan, int trainTargetWaypointIndex) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.tripPlan = tripPlan;

            tripPlanEditor.Initialize();
            tripPlanEditor.ViewOnly = true;
            tripPlanEditor.Train = train;
            tripPlanEditor.TrainTargetWaypoint = trainTargetWaypointIndex;
            tripPlanEditor.TripPlan = tripPlan;

            Text = "Trip plan for " + train.DisplayName;
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

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TripPlanViewing_Closed(object? sender, System.EventArgs e) => Dispose(true);

        private void ButtonSave_Click(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("save-trip-plan", tripPlan, this));
        }
    }
}
