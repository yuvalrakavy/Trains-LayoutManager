using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for GetTargetSpeed.
    /// </summary>
    public class GetTargetSpeed : Form {
        private Button buttonOK;
        private Button buttonCancel;
        private System.Windows.Forms.TrackBar trackBarTargetSpeed;
        private NumericUpDown numericUpDownTargetSpeed;
        private Label labelLowerLimit;
        private Label labelUpperLimit;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.trackBarTargetSpeed = new System.Windows.Forms.TrackBar();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.numericUpDownTargetSpeed = new NumericUpDown();
            this.labelLowerLimit = new Label();
            this.labelUpperLimit = new Label();
            ((System.ComponentModel.ISupportInitialize)this.trackBarTargetSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownTargetSpeed).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarTargetSpeed
            // 
            this.trackBarTargetSpeed.Location = new System.Drawing.Point(16, 8);
            this.trackBarTargetSpeed.Name = "trackBarTargetSpeed";
            this.trackBarTargetSpeed.Size = new System.Drawing.Size(224, 45);
            this.trackBarTargetSpeed.TabIndex = 0;
            this.trackBarTargetSpeed.Scroll += this.trackBarTargetSpeed_Scroll;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(188, 68);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(56, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(249, 68);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // numericUpDownTargetSpeed
            // 
            this.numericUpDownTargetSpeed.Location = new System.Drawing.Point(256, 11);
            this.numericUpDownTargetSpeed.Name = "numericUpDownTargetSpeed";
            this.numericUpDownTargetSpeed.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownTargetSpeed.TabIndex = 3;
            this.numericUpDownTargetSpeed.ValueChanged += this.numericUpDownTargetSpeed_ValueChanged;
            // 
            // labelLowerLimit
            // 
            this.labelLowerLimit.Location = new System.Drawing.Point(21, 40);
            this.labelLowerLimit.Name = "labelLowerLimit";
            this.labelLowerLimit.Size = new System.Drawing.Size(16, 23);
            this.labelLowerLimit.TabIndex = 1;
            this.labelLowerLimit.Text = "1";
            this.labelLowerLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelUpperLimit
            // 
            this.labelUpperLimit.Location = new System.Drawing.Point(208, 40);
            this.labelUpperLimit.Name = "labelUpperLimit";
            this.labelUpperLimit.Size = new System.Drawing.Size(32, 23);
            this.labelUpperLimit.TabIndex = 2;
            this.labelUpperLimit.Text = "1";
            this.labelUpperLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GetTargetSpeed
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(314, 96);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.labelUpperLimit,
                                                                          this.labelLowerLimit,
                                                                          this.numericUpDownTargetSpeed,
                                                                          this.buttonOK,
                                                                          this.trackBarTargetSpeed,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GetTargetSpeed";
            this.Text = "Set Train Target Speed";
            ((System.ComponentModel.ISupportInitialize)this.trackBarTargetSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownTargetSpeed).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private void trackBarTargetSpeed_Scroll(object sender, System.EventArgs e) {
            if (trackBarTargetSpeed.Value != targetSpeed) {
                targetSpeed = trackBarTargetSpeed.Value;
                numericUpDownTargetSpeed.Value = targetSpeed;
            }
        }

        private void numericUpDownTargetSpeed_ValueChanged(object sender, System.EventArgs e) {
            if (numericUpDownTargetSpeed.Value != targetSpeed) {
                targetSpeed = (int)numericUpDownTargetSpeed.Value;
                trackBarTargetSpeed.Value = targetSpeed;
            }
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            train.TargetSpeed = (int)numericUpDownTargetSpeed.Value;
            DialogResult = DialogResult.OK;

            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
