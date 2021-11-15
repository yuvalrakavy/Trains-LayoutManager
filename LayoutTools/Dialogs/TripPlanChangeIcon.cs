using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanChangeIcon.
    /// </summary>
    public partial class TripPlanChangeIcon : Form {
        private readonly TripPlanInfo tripPlan;

        public TripPlanChangeIcon(TripPlanInfo tripPlan, TripPlanIconListInfo tripPlanIconList) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.tripPlan = tripPlan;

            labelTripPlanName.Text = tripPlan.Name;
            selectTripPlanIcon.IconList = tripPlanIconList;
            selectTripPlanIcon.SelectedID = tripPlan.IconId;
            selectTripPlanIcon.Initialize();
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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            tripPlan.IconId = selectTripPlanIcon.SelectedID;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
