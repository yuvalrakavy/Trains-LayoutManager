using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for EditMotionRamp.
    /// </summary>
    public partial class EditMotionRamp : Form {
        public EditMotionRamp(MotionRampInfo ramp) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

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


        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (!motionRampEditor.ValidateValues())
                return;

            motionRampEditor.Commit();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
