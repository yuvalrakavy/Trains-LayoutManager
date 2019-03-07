using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Edit the parameters of MotionRampInfo
    /// </summary>
    public class MotionRampEditor : System.Windows.Forms.UserControl {
        private GroupBox groupBox1;
        private RadioButton radioButtonSpeedChangeLength;
        private TextBox textBoxSpeedChangeLength;
        private Label label1;
        private RadioButton radioButtonSpeedChangeRate;
        private TextBox textBoxSpeedChangeRate;
        private Label label2;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private RadioButton radioButtonLocomotiveHardware;
        MotionRampInfo ramp;

        public MotionRampEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public MotionRampInfo Ramp {
            get {
                return ramp;
            }

            set {
                ramp = value;
                if (ramp != null)
                    initialize();
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
                    ramp.RampType = MotionRampType.RateFixed;
                    ramp.SpeedChangeRate = int.Parse(textBoxSpeedChangeRate.Text);
                }
                else if (radioButtonSpeedChangeLength.Checked) {
                    ramp.RampType = MotionRampType.TimeFixed;
                    ramp.MotionTime = (int)Math.Round(1000 * double.Parse(textBoxSpeedChangeLength.Text));
                }
                else if (radioButtonLocomotiveHardware.Checked)
                    ramp.RampType = MotionRampType.LocomotiveHardware;
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

        private void initialize() {
            if (ramp.RampType == MotionRampType.RateFixed) {
                radioButtonSpeedChangeRate.Checked = true;
                textBoxSpeedChangeRate.Text = ramp.SpeedChangeRate.ToString();
            }
            else if (ramp.RampType == MotionRampType.TimeFixed) {
                double speedChangeLength = (double)ramp.MotionTime / 1000.0;

                radioButtonSpeedChangeLength.Checked = true;
                textBoxSpeedChangeLength.Text = speedChangeLength.ToString();
            }
            else if (ramp.RampType == MotionRampType.LocomotiveHardware)
                radioButtonLocomotiveHardware.Checked = true;

            updateButtons(null, null);
        }

        private void updateButtons(object sender, EventArgs e) {
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.label1 = new Label();
            this.textBoxSpeedChangeLength = new TextBox();
            this.radioButtonSpeedChangeLength = new RadioButton();
            this.radioButtonSpeedChangeRate = new RadioButton();
            this.textBoxSpeedChangeRate = new TextBox();
            this.label2 = new Label();
            this.radioButtonLocomotiveHardware = new RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.radioButtonLocomotiveHardware,
                                                                                    this.label1,
                                                                                    this.textBoxSpeedChangeLength,
                                                                                    this.radioButtonSpeedChangeLength,
                                                                                    this.radioButtonSpeedChangeRate,
                                                                                    this.textBoxSpeedChangeRate,
                                                                                    this.label2});
            this.groupBox1.Location = new System.Drawing.Point(7, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set speed change parameter:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(128, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "seconds";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxSpeedChangeLength
            // 
            this.textBoxSpeedChangeLength.Location = new System.Drawing.Point(74, 18);
            this.textBoxSpeedChangeLength.Name = "textBoxSpeedChangeLength";
            this.textBoxSpeedChangeLength.Size = new System.Drawing.Size(48, 20);
            this.textBoxSpeedChangeLength.TabIndex = 1;
            this.textBoxSpeedChangeLength.Text = "";
            this.textBoxSpeedChangeLength.TextChanged += new System.EventHandler(this.textBoxSpeedChangeLength_TextChanged);
            // 
            // radioButtonSpeedChangeLength
            // 
            this.radioButtonSpeedChangeLength.Location = new System.Drawing.Point(8, 18);
            this.radioButtonSpeedChangeLength.Name = "radioButtonSpeedChangeLength";
            this.radioButtonSpeedChangeLength.Size = new System.Drawing.Size(61, 20);
            this.radioButtonSpeedChangeLength.TabIndex = 0;
            this.radioButtonSpeedChangeLength.Text = "Length:";
            this.radioButtonSpeedChangeLength.CheckedChanged += new System.EventHandler(this.radioButtonSpeedChangeLength_CheckedChanged);
            // 
            // radioButtonSpeedChangeRate
            // 
            this.radioButtonSpeedChangeRate.Location = new System.Drawing.Point(8, 40);
            this.radioButtonSpeedChangeRate.Name = "radioButtonSpeedChangeRate";
            this.radioButtonSpeedChangeRate.Size = new System.Drawing.Size(61, 20);
            this.radioButtonSpeedChangeRate.TabIndex = 3;
            this.radioButtonSpeedChangeRate.Text = "Rate:";
            this.radioButtonSpeedChangeRate.CheckedChanged += new System.EventHandler(this.radioButtonSpeedChangeRate_CheckedChanged);
            // 
            // textBoxSpeedChangeRate
            // 
            this.textBoxSpeedChangeRate.Location = new System.Drawing.Point(74, 40);
            this.textBoxSpeedChangeRate.Name = "textBoxSpeedChangeRate";
            this.textBoxSpeedChangeRate.Size = new System.Drawing.Size(48, 20);
            this.textBoxSpeedChangeRate.TabIndex = 4;
            this.textBoxSpeedChangeRate.Text = "";
            this.textBoxSpeedChangeRate.TextChanged += new System.EventHandler(this.textBoxSpeedChangeRate_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(128, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "steps/second";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButtonLocomotiveHardware
            // 
            this.radioButtonLocomotiveHardware.Location = new System.Drawing.Point(8, 61);
            this.radioButtonLocomotiveHardware.Name = "radioButtonLocomotiveHardware";
            this.radioButtonLocomotiveHardware.Size = new System.Drawing.Size(152, 20);
            this.radioButtonLocomotiveHardware.TabIndex = 6;
            this.radioButtonLocomotiveHardware.Text = "Locomotive own settings";
            this.radioButtonLocomotiveHardware.CheckedChanged += new System.EventHandler(this.radioButtonLocomotiveHardware_CheckedChanged);
            // 
            // MotionRampEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.groupBox1});
            this.Name = "MotionRampEditor";
            this.Size = new System.Drawing.Size(218, 98);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private void textBoxSpeedChangeLength_TextChanged(object sender, System.EventArgs e) {
            if (textBoxSpeedChangeLength.Text != "")
                radioButtonSpeedChangeLength.Checked = true;
        }

        private void textBoxSpeedChangeRate_TextChanged(object sender, System.EventArgs e) {
            if (textBoxSpeedChangeRate.Text != "")
                radioButtonSpeedChangeRate.Checked = true;

        }

        private void radioButtonSpeedChangeLength_CheckedChanged(object sender, System.EventArgs e) {
            if (radioButtonSpeedChangeLength.Checked)
                textBoxSpeedChangeRate.Text = "";
        }

        private void radioButtonSpeedChangeRate_CheckedChanged(object sender, System.EventArgs e) {
            if (radioButtonSpeedChangeRate.Checked)
                textBoxSpeedChangeLength.Text = "";
        }

        private void radioButtonLocomotiveHardware_CheckedChanged(object sender, System.EventArgs e) {
            if (radioButtonLocomotiveHardware.Checked) {
                textBoxSpeedChangeLength.Text = "";
                textBoxSpeedChangeRate.Text = "";
            }
        }
    }
}
