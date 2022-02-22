using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for MotionRampDefinition.
    /// </summary>
    public partial class MotionRampDefinition : Form {

        private readonly MotionRampInfo ramp;

        public MotionRampDefinition(MotionRampInfo ramp) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ramp = ramp;

            textBoxName.Text = ramp.Name;
            checkBoxSHowInTrainControllerDialog.Checked = ramp.UseInTrainControllerDialog;

            motionRampEditor.Ramp = ramp;
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
            if (textBoxName.Text.Trim() == "") {
                MessageBox.Show(this, "You must specify name", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (!motionRampEditor.ValidateValues())
                return;

            ramp.Name = textBoxName.Text;
            ramp.UseInTrainControllerDialog = checkBoxSHowInTrainControllerDialog.Checked;
            motionRampEditor.Commit();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
