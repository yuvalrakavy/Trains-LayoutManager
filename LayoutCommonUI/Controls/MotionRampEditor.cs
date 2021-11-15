using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Edit the parameters of MotionRampInfo
    /// </summary>
    public partial class MotionRampEditor : UserControl {
        private MotionRampInfo? ramp;

        public MotionRampEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public MotionRampInfo Ramp {
            get {
                return Ensure.NotNull<MotionRampInfo>(ramp, nameof(ramp));
            }

            set {
                ramp = value;
                if (ramp != null)
                    Initialize();
            }
        }

        public bool ValidateValues() {
            if (radioButtonSpeedChangeRate.Checked) {
                try {
                    int.Parse(textBoxSpeedChangeRate.Text);
                }
                catch (FormatException) {
                    MessageBox.Show(this, "Invalid speed change rate value", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxSpeedChangeRate.Focus();
                    return false;
                }
            }
            else if (radioButtonSpeedChangeLength.Checked) {
                try {
                    double.Parse(textBoxSpeedChangeLength.Text);
                }
                catch (FormatException) {
                    MessageBox.Show(this, "Invalid speed change length value", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxSpeedChangeLength.Focus();
                    return false;
                }
            }
            else if (!radioButtonLocomotiveHardware.Checked) {
                MessageBox.Show(this, "You have not chosen which parameter to set", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateValues())
                return false;
            else {
                if (radioButtonSpeedChangeRate.Checked) {
                    Ramp.RampType = MotionRampType.RateFixed;
                    Ramp.SpeedChangeRate = int.Parse(textBoxSpeedChangeRate.Text);
                }
                else if (radioButtonSpeedChangeLength.Checked) {
                    Ramp.RampType = MotionRampType.TimeFixed;
                    Ramp.MotionTime = (int)Math.Round(1000 * double.Parse(textBoxSpeedChangeLength.Text));
                }
                else if (radioButtonLocomotiveHardware.Checked)
                    Ramp.RampType = MotionRampType.LocomotiveHardware;
                else
                    return false;

                return true;
            }
        }

        public void CopyFrom(MotionRampInfo otherRamp) {
            if (otherRamp.RampType == MotionRampType.TimeFixed) {
                double t = Math.Round((double)otherRamp.MotionTime / 1000.0);

                radioButtonSpeedChangeLength.Checked = true;
                textBoxSpeedChangeLength.Text = t.ToString();
            }
            else if (otherRamp.RampType == MotionRampType.RateFixed) {
                radioButtonSpeedChangeRate.Checked = true;
                textBoxSpeedChangeRate.Text = otherRamp.SpeedChangeRate.ToString();
            }
            else if (otherRamp.RampType == MotionRampType.LocomotiveHardware)
                radioButtonLocomotiveHardware.Checked = true;
            else
                throw new ApplicationException("Invalid motion ramp type " + otherRamp.RampType);
        }

        private void Initialize() {
            if (Ramp.RampType == MotionRampType.RateFixed) {
                radioButtonSpeedChangeRate.Checked = true;
                textBoxSpeedChangeRate.Text = Ramp.SpeedChangeRate.ToString();
            }
            else if (Ramp.RampType == MotionRampType.TimeFixed) {
                double speedChangeLength = (double)Ramp.MotionTime / 1000.0;

                radioButtonSpeedChangeLength.Checked = true;
                textBoxSpeedChangeLength.Text = speedChangeLength.ToString();
            }
            else if (Ramp.RampType == MotionRampType.LocomotiveHardware)
                radioButtonLocomotiveHardware.Checked = true;

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
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


        private void TextBoxSpeedChangeLength_TextChanged(object? sender, EventArgs e) {
            if (textBoxSpeedChangeLength.Text != "")
                radioButtonSpeedChangeLength.Checked = true;
        }

        private void TextBoxSpeedChangeRate_TextChanged(object? sender, EventArgs e) {
            if (textBoxSpeedChangeRate.Text != "")
                radioButtonSpeedChangeRate.Checked = true;
        }

        private void RadioButtonSpeedChangeLength_CheckedChanged(object? sender, EventArgs e) {
            if (radioButtonSpeedChangeLength.Checked)
                textBoxSpeedChangeRate.Text = "";
        }

        private void RadioButtonSpeedChangeRate_CheckedChanged(object? sender, EventArgs e) {
            if (radioButtonSpeedChangeRate.Checked)
                textBoxSpeedChangeLength.Text = "";
        }

        private void RadioButtonLocomotiveHardware_CheckedChanged(object? sender, EventArgs e) {
            if (radioButtonLocomotiveHardware.Checked) {
                textBoxSpeedChangeLength.Text = "";
                textBoxSpeedChangeRate.Text = "";
            }
        }
    }
}
