using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for GetTargetSpeed.
    /// </summary>
    public partial class GetTargetSpeed : Form {
        private int targetSpeed;
        private readonly TrainCommonInfo train;

        public GetTargetSpeed(TrainCommonInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;
            targetSpeed = train.TargetSpeed;

            trackBarTargetSpeed.Minimum = 1;
            trackBarTargetSpeed.Maximum = LayoutModel.Instance.LogicalSpeedSteps;
            trackBarTargetSpeed.Value = targetSpeed;

            numericUpDownTargetSpeed.Value = targetSpeed;
            numericUpDownTargetSpeed.Minimum = 1;
            numericUpDownTargetSpeed.Maximum = LayoutModel.Instance.LogicalSpeedSteps;

            labelUpperLimit.Text = LayoutModel.Instance.LogicalSpeedSteps.ToString();
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

        private void TrackBarTargetSpeed_Scroll(object? sender, System.EventArgs e) {
            if (trackBarTargetSpeed.Value != targetSpeed) {
                targetSpeed = trackBarTargetSpeed.Value;
                numericUpDownTargetSpeed.Value = targetSpeed;
            }
        }

        private void NumericUpDownTargetSpeed_ValueChanged(object? sender, System.EventArgs e) {
            if (numericUpDownTargetSpeed.Value != targetSpeed) {
                targetSpeed = (int)numericUpDownTargetSpeed.Value;
                trackBarTargetSpeed.Value = targetSpeed;
            }
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            train.TargetSpeed = (int)numericUpDownTargetSpeed.Value;
            DialogResult = DialogResult.OK;

            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
