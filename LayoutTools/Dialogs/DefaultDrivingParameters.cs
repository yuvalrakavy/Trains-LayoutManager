using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for DefaultDrivingParameters.
    /// </summary>
    public partial class DefaultDrivingParameters : Form {

        public DefaultDrivingParameters() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            DefaultDriverParametersInfo d = LayoutModel.StateManager.DefaultDriverParameters;

            textBoxSpeedLimit.Text = d.SpeedLimit.ToString();
            labelSpeedLimitValues.Text = "(1-" + LayoutModel.Instance.LogicalSpeedSteps + ")";

            motionRampWithCopyEditorAcceleration.Ramp = d.AccelerationProfile ?? MotionRampInfo.Default;
            motionRampWithCopyEditorDeceleration.Ramp = d.DecelerationProfile ?? MotionRampInfo.Default;

            textBoxSlowdown.Text = d.SlowdownSpeed.ToString();
            labelSlowdownValues.Text = "(1-" + LayoutModel.Instance.LogicalSpeedSteps + ")";

            motionRampWithCopyEditorSlowdown.Ramp = d.SlowdownProfile ?? MotionRampInfo.Default;
            motionRampWithCopyEditorStop.Ramp = d.StopProfile ?? MotionRampInfo.Default;
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

        private bool ValidateSpeed(TextBox t, string title) {
            int speed;

            try {
                speed = int.Parse(t.Text);
            }
            catch (FormatException) {
                MessageBox.Show(this, "Invalid value for " + title, "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                t.Focus();
                return false;
            }

            if (speed < 1 || speed > LayoutModel.Instance.LogicalSpeedSteps) {
                MessageBox.Show(this, "Invalid value for " + title + ". Must be between 1 and " + LayoutModel.Instance.LogicalSpeedSteps + ".", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                t.Focus();
                return false;
            }

            return true;
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (!ValidateSpeed(textBoxSpeedLimit, "speed limit"))
                return;

            if (!motionRampWithCopyEditorAcceleration.ValidateValues() ||
                !motionRampWithCopyEditorDeceleration.ValidateValues())
                return;

            if (!ValidateSpeed(textBoxSlowdown, "slow down speed"))
                return;

            if (!motionRampWithCopyEditorSlowdown.ValidateValues() ||
                !motionRampWithCopyEditorStop.ValidateValues())
                return;

            motionRampWithCopyEditorAcceleration.Commit();
            motionRampWithCopyEditorDeceleration.Commit();
            motionRampWithCopyEditorSlowdown.Commit();
            motionRampWithCopyEditorStop.Commit();

            DefaultDriverParametersInfo d = LayoutModel.StateManager.DefaultDriverParameters;

            d.SpeedLimit = int.Parse(textBoxSpeedLimit.Text);
            d.SlowdownSpeed = int.Parse(textBoxSlowdown.Text);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
