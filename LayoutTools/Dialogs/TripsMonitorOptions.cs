using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripsMonitorOptions.
    /// </summary>
    public class TripsMonitorOptions : Form {
        private CheckBox checkBoxEnableAutoClear;
        private Label labelAutoClear1;
        private NumericUpDown numericUpDownAutoClearTimeout;
        private Label labelAutoClear2;
        private Button buttonOk;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public TripsMonitorOptions(int autoClearTimeout) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (autoClearTimeout < 0) {
                checkBoxEnableAutoClear.Checked = false;
                numericUpDownAutoClearTimeout.Value = 15;
            }
            else {
                checkBoxEnableAutoClear.Checked = true;
                numericUpDownAutoClearTimeout.Value = autoClearTimeout;
            }

            updateButtons(null, null);
        }

        private void updateButtons(object sender, EventArgs e) {
            if (checkBoxEnableAutoClear.Checked) {
                labelAutoClear1.Enabled = true;
                labelAutoClear2.Enabled = true;
                numericUpDownAutoClearTimeout.Enabled = true;
            }
            else {
                labelAutoClear1.Enabled = false;
                labelAutoClear2.Enabled = false;
                numericUpDownAutoClearTimeout.Enabled = false;
            }
        }

        public int AutoClearTimeout {
            get {
                if (checkBoxEnableAutoClear.Checked)
                    return (int)numericUpDownAutoClearTimeout.Value;
                else
                    return -1;
            }
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
            this.checkBoxEnableAutoClear = new CheckBox();
            this.labelAutoClear1 = new Label();
            this.numericUpDownAutoClearTimeout = new NumericUpDown();
            this.labelAutoClear2 = new Label();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoClearTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxEnableAutoClear
            // 
            this.checkBoxEnableAutoClear.Location = new System.Drawing.Point(8, 8);
            this.checkBoxEnableAutoClear.Name = "checkBoxEnableAutoClear";
            this.checkBoxEnableAutoClear.Size = new System.Drawing.Size(216, 24);
            this.checkBoxEnableAutoClear.TabIndex = 0;
            this.checkBoxEnableAutoClear.Text = "Automatically clear completed trips";
            this.checkBoxEnableAutoClear.CheckedChanged += new System.EventHandler(this.updateButtons);
            // 
            // labelAutoClear1
            // 
            this.labelAutoClear1.Location = new System.Drawing.Point(24, 32);
            this.labelAutoClear1.Name = "labelAutoClear1";
            this.labelAutoClear1.Size = new System.Drawing.Size(152, 23);
            this.labelAutoClear1.TabIndex = 1;
            this.labelAutoClear1.Text = "Remove completed trip after ";
            this.labelAutoClear1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownAutoClearTimeout
            // 
            this.numericUpDownAutoClearTimeout.Location = new System.Drawing.Point(177, 33);
            this.numericUpDownAutoClearTimeout.Maximum = new System.Decimal(new int[] {
                                                                                          30000,
                                                                                          0,
                                                                                          0,
                                                                                          0});
            this.numericUpDownAutoClearTimeout.Name = "numericUpDownAutoClearTimeout";
            this.numericUpDownAutoClearTimeout.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownAutoClearTimeout.TabIndex = 2;
            // 
            // labelAutoClear2
            // 
            this.labelAutoClear2.Location = new System.Drawing.Point(227, 32);
            this.labelAutoClear2.Name = "labelAutoClear2";
            this.labelAutoClear2.TabIndex = 3;
            this.labelAutoClear2.Text = "Seconds";
            this.labelAutoClear2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOk.Location = new System.Drawing.Point(144, 96);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(224, 96);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // TripsMonitorOptions
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonOk;
            this.ClientSize = new System.Drawing.Size(304, 126);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOk,
                                                                          this.labelAutoClear2,
                                                                          this.numericUpDownAutoClearTimeout,
                                                                          this.labelAutoClear1,
                                                                          this.checkBoxEnableAutoClear,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TripsMonitorOptions";
            this.ShowInTaskbar = false;
            this.Text = "Trip Viewer Options";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoClearTimeout)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOk_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
