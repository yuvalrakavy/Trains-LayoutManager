using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for TestControl.
    /// </summary>
    public partial class TestControl : Form {
        public TestControl() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            selectTripPlanIcon1.IconList = LayoutModel.Instance.TripPlanIconList;
            selectTripPlanIcon1.Initialize();
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
            Close();
        }
    }
}
