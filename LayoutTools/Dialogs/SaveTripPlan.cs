using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SaveTripPlan.
    /// </summary>
    public partial class SaveTripPlan : Form {

        public SaveTripPlan() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            selectTripPlanIcon.IconList = LayoutModel.Instance.TripPlanIconList;
            selectTripPlanIcon.Initialize();
        }

        public string TripPlanName {
            get {
                return textBoxName.Text;
            }

            set {
                textBoxName.Text = value;
            }
        }

        public Guid IconID => selectTripPlanIcon.SelectedID;

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

        private void ButtonSave_Click(object? sender, System.EventArgs e) {
            if (textBoxName.Text.Trim() == "") {
                MessageBox.Show(this, "Please specify name for the saved trip-plan", "Missing Trip-plan Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (selectTripPlanIcon.IconList.LargeIconImageList.Images.Count > 0 && selectTripPlanIcon.SelectedIndex < 0) {
                MessageBox.Show(this, "Please select an icon for the save trip-plan", "No Trip-plan Icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectTripPlanIcon.Focus();
                return;
            }

            if (selectTripPlanIcon.IconList.IconListModified)
                LayoutModel.WriteModelXmlInfo();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            if (selectTripPlanIcon.IconList.IconListModified)
                LayoutModel.WriteModelXmlInfo();
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
